using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using YOLOv4MLNet.DataStructures;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Globalization;
using System.Collections.Concurrent;
//using System.Collections.Generic;


// TODO: split code to libs
// console input
// more tests

namespace YOLOv4MLNet
{
    class Detector
    {
        // model is available here:
        // https://github.com/onnx/models/tree/master/vision/object_detection_segmentation/yolov4
        const string modelPath = @"C:\yolov4.onnx";

        //const string imageFolder = @"D:\models\Assets\Images";

        //public string imageFolder { get; set; }

        const string imageOutputFolder = @"D:\models\Assets\Output";

        static readonly string[] classesNames = new string[] { "person", "bicycle", "car", "motorbike", 
            "aeroplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", 
            "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", 
            "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", 
            "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard",
            "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", 
            "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", 
            "chair", "sofa", "pottedplant", "bed", "diningtable", "toilet", "tvmonitor", "laptop", "mouse", 
            "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", 
            "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };

        //public Detector (string folder)
        //{
        //    imageFolder = folder;
        //}

        public void DrawPredictions(YoloV4Prediction res)
        {

        }

        public static void printResult(YoloV4Result res)
        {
            //var x1 = res.BBox[0];
            //var y1 = res.BBox[1];
            //var x2 = res.BBox[2];
            //var y2 = res.BBox[3];
            Console.WriteLine($"{res.Label}");
        }

        public static async Task DetectImage(string imageFolder)
        {
            Directory.CreateDirectory(imageOutputFolder);
            MLContext mlContext = new MLContext();

            // model is available here:
            // https://github.com/onnx/models/tree/master/vision/object_detection_segmentation/yolov4

            // Define scoring pipeline
            var pipeline = mlContext.Transforms.ResizeImages(inputColumnName: "bitmap", 
                outputColumnName: "input_1:0", imageWidth: 416, imageHeight: 416, resizing: ResizingKind.IsoPad)
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    shapeDictionary: new Dictionary<string, int[]>()
                    {
                        { "input_1:0", new[] { 1, 416, 416, 3 } },
                        { "Identity:0", new[] { 1, 52, 52, 3, 85 } },
                        { "Identity_1:0", new[] { 1, 26, 26, 3, 85 } },
                        { "Identity_2:0", new[] { 1, 13, 13, 3, 85 } },
                    },
                    inputColumnNames: new[]
                    {
                        "input_1:0"
                    },
                    outputColumnNames: new[]
                    {
                        "Identity:0",
                        "Identity_1:0",
                        "Identity_2:0"
                    },
                    modelFile: modelPath, recursionLimit: 100));

            // Fit on empty list to obtain input data schema
            var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloV4BitmapData>()));

            // Create prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);

            // save model
            //mlContext.Model.Save(model, predictionEngine.OutputSchema, Path.ChangeExtension(modelPath, "zip"));
            //ConcurrentBag<string> processedImages = new ConcurrentBag<string>();
            ConcurrentBag<string> detectedObjects = new ConcurrentBag<string>();
            var sw = new Stopwatch();
            sw.Start();


            string[] imageNames = Directory.GetFiles(imageFolder);
            ProcessedImages processedImages = new ProcessedImages(imageNames.Length);
            object locker = new object();
            
            var ab = new ActionBlock<string>(async image => {
                YoloV4Prediction predict;
                lock (locker)
                {
                    using (var bitmap = new Bitmap(Image.FromFile(Path.Combine(image))))
                    {
                        // predict
                        predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                    }
                    processedImages.AddToBag(image);
                }
                var results = predict.GetResults(classesNames, 0.3f, 0.7f);
                foreach (var res in results)
                {
                    //printResult(res);
                    detectedObjects.Add(res.Label);
                }
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 4
            });
            Parallel.For(0, imageNames.Length, i => ab.Post(imageNames[i]));
            ab.Complete();

            await ab.Completion;

            /*
            foreach (string imageName in new string[] { "kite.jpg", "dog_cat.jpg", "cars road.jpg", "ski.jpg", "ski2.jpg" })
            {
                using (var bitmap = new Bitmap(Image.FromFile(Path.Combine(imageFolder, imageName))))
                {
                    // predict
                    var predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                    var results = predict.GetResults(classesNames, 0.3f, 0.7f);

                    using (var g = Graphics.FromImage(bitmap))
                    {
                        foreach (var res in results)
                        {
                            // draw predictions
                            var x1 = res.BBox[0];
                            var y1 = res.BBox[1];
                            var x2 = res.BBox[2];
                            var y2 = res.BBox[3];
                            g.DrawRectangle(Pens.Red, x1, y1, x2 - x1, y2 - y1);
                            using (var brushes = new SolidBrush(Color.FromArgb(50, Color.Red)))
                            {
                                g.FillRectangle(brushes, x1, y1, x2 - x1, y2 - y1);
                            }

                            g.DrawString(res.Label + " " + res.Confidence.ToString("0.00"),
                                         new Font("Arial", 12), Brushes.Blue, new PointF(x1, y1));
                        }
                        bitmap.Save(Path.Combine(imageOutputFolder, Path.ChangeExtension(imageName, "_processed" + Path.GetExtension(imageName))));
                    }
                }
            }
            */

            sw.Stop();




            Console.WriteLine($"Done in {sw.ElapsedMilliseconds}ms.");
            foreach (string obj in detectedObjects)
            {
                Console.WriteLine(obj);
            }
            Console.WriteLine(detectedObjects.Count.ToString());
        }
    }
}
