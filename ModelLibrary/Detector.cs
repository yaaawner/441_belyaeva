using System;
using System.Collections.Generic;
using Microsoft.ML;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using YOLOv4MLNet.DataStructures;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Concurrent;

namespace ModelLibrary
{
    //var bufferBlock = new BufferBlock<int>();
    public class Detector
    {
        // model is available here:
        // https://github.com/onnx/models/tree/master/vision/object_detection_segmentation/yolov4
        const string modelPath = @"C:\yolov4.onnx";

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

        public static BufferBlock<string> bufferBlock = new BufferBlock<string>();

        public static BufferBlock<(string, string)> resultBufferBlock = new BufferBlock<(string, string)>();

        /*
        private static async Task Consumer()
        {
            while (true)
            {
                Console.WriteLine(await bufferBlock.ReceiveAsync());
            }
        }
        */
        //Dictionary<string, ConcurrentBag<string>> recognizedObjects = new Dictionary<string, ConcurrentBag<string>>();

        public static async Task DetectImage(string imageFolder)
        {
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

            ConcurrentBag<YoloV4Result> detectedObjects = new ConcurrentBag<YoloV4Result>();
            string[] imageNames = Directory.GetFiles(imageFolder);
            ProcessedImages processedImages = new ProcessedImages(imageNames.Length);

            Dictionary<string, ConcurrentBag<string>> recognizedObjects = new Dictionary<string, ConcurrentBag<string>>();

            foreach (string name in classesNames) {
                recognizedObjects.Add(name, new ConcurrentBag<string>());
            }

            object locker = new object();

            var sw = new Stopwatch();
            sw.Start();
            
            var ab = new ActionBlock<string>(async image => {
                YoloV4Prediction predict;
                lock (locker)
                {
                    var bitmap = new Bitmap(Image.FromFile(Path.Combine(image)));
                    predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                    //processedImages.Add(image);
                }

                var results = predict.GetResults(classesNames, 0.3f, 0.7f);
                foreach (var res in results)
                {
                    recognizedObjects[res.Label].Add(image);

                    await resultBufferBlock.SendAsync((res.Label, image));
                }
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

            Parallel.For(0, imageNames.Length, i => ab.Post(imageNames[i]));
            ab.Complete();
            await ab.Completion;
            

            // 3 action blocks
            /*
            var bitmapBlock = new TransformBlock<string, Bitmap>(async image =>
            {
                var bitmap = new Bitmap(Image.FromFile(Path.Combine(image)));
                return bitmap;
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

            var predictBlock = new TransformBlock<Bitmap, YoloV4Prediction>(async bitmap =>
            {
                YoloV4Prediction predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                await bufferBlock.SendAsync(processedImages.Add(bitmap));
                return predict;
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,
            });

            var resultBlock = new ActionBlock<YoloV4Prediction>(async predict =>
            {
                var results = predict.GetResults(classesNames, 0.3f, 0.7f);
                foreach (var res in results)
                {
                    detectedObjects.Add(res);
                    var x1 = res.BBox[0];
                    var y1 = res.BBox[1];
                    var x2 = res.BBox[2];
                    var y2 = res.BBox[3];
                    await bufferBlock.SendAsync($"[left,top,right,bottom]:[{x1}, {y1}, {x2}, {y2}] object {res.Label}");
                    //await bufferBlock.SendAsync(detectedObjects.Percent());
                    //Console.WriteLine($"[left,top,right,bottom]:[{x1}, {y1}, {x2}, {y2}] object {res.Label}");
                }
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

            var link = new DataflowLinkOptions { PropagateCompletion = true };
            bitmapBlock.LinkTo(predictBlock, link);
            predictBlock.LinkTo(resultBlock, link);

            Parallel.For(0, imageNames.Length, i => bitmapBlock.Post(imageNames[i]));
            bitmapBlock.Complete();
            await resultBlock.Completion;
            */
            sw.Stop();

            //Console.WriteLine($"Done in {sw.ElapsedMilliseconds}ms.");
            /*
            Console.WriteLine("List of finding objects: ");
            foreach (var obj in detectedObjects)
            {
                var x1 = obj.BBox[0];
                var y1 = obj.BBox[1];
                var x2 = obj.BBox[2];
                var y2 = obj.BBox[3];
                Console.WriteLine($"[left,top,right,bottom]:[{x1}, {y1}, {x2}, {y2}] object {obj.Label}");
            }
            Console.WriteLine($"Total number of objects: {detectedObjects.Count}");
            */
            await Detector.bufferBlock.SendAsync($"Total number of objects: {detectedObjects.Count}");
            await Detector.bufferBlock.SendAsync("end");

            foreach (KeyValuePair<string, ConcurrentBag<string>> pairs in recognizedObjects)
            {
                Console.WriteLine(pairs.Key + ": " + pairs.Value.ToString());
            }
        }
    }
}
