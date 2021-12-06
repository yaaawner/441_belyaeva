using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ModelLibrary;
using System.Collections.Generic;
using Microsoft.ML;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using YOLOv4MLNet.DataStructures;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ConsoleApp3
{
    class Program
    {
        private static async Task Consumer()
        {
            string type;
            string image;
            Bitmap bitmap;
            float[] BBox;

            using (ResultContext db = new ResultContext())
            {
                while (true)
                {
                    (type, image, bitmap, BBox) = await Detector.resultBufferBlock.ReceiveAsync();
                    if (type == "end")
                    {
                        db.SaveChanges();
                        break;
                    }

                    bool flag = true;
                    foreach (var res in db.Results)
                    {
                        if (res.Type == type)
                        {
                            bool flagimg = true;
                            foreach(var obj in res.DetectedObjects)
                            {
                                if (obj.x1 == BBox[0] && obj.y1 == BBox[1] && obj.x2 == BBox[2] && obj.y2 == BBox[3])
                                {
                                    if (obj.BitmapImage == ImageToByte2(bitmap))
                                    {
                                        flagimg = false;
                                        break;
                                    }
                                }
                            }
                            if (flagimg)
                            {
                                res.DetectedObjects.Add(new DetectedObject
                                {
                                    Path = image,
                                    x1 = BBox[0],
                                    y1 = BBox[1],
                                    x2 = BBox[2],
                                    y2 = BBox[3],
                                    BitmapImage = ImageToByte2(bitmap)
                                });
                            }
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        Results result = new Results { Type = type };
                        result.DetectedObjects = new List<DetectedObject>();
                        DetectedObject objects = new DetectedObject
                        {
                            Path = image,
                            x1 = BBox[0],
                            y1 = BBox[1],
                            x2 = BBox[2],
                            y2 = BBox[3],
                            BitmapImage = ImageToByte2(bitmap)
                        };
                        result.DetectedObjects.Add(objects);
                        db.Results.Add(result);
                        //db.Results

                    }

                    db.SaveChanges();
                }
                foreach (var res in db.Results.Include(a => a.DetectedObjects))
                {
                    Console.WriteLine($"{res.ResultsId} {res.Type}");
                    // db.Entry(a2).Collection(a => a.Books).Load();
                    foreach (var d in res.DetectedObjects)
                        Console.WriteLine($"       {d.DetectedObjectId} {d.Path}");
                }
                
                // Clear
                foreach (var res in db.Results)
                {
                    db.Results.Remove(res);
                }

            }
        }
        static async Task Main()
        {
            //var bufferBlock = new BufferBlock<string>();
            Console.WriteLine("Write input path: ");
            //string imageFolder = Console.ReadLine();
            string imageFolder = @"D:\models\Assets\Images";
            //await Detector.DetectImage(imageFolder);

            await Task.WhenAll(Detector.DetectImage(imageFolder), Consumer());

            /*
            while (Detector.bufferBlock.TryReceive(out string value))
            {
                Console.WriteLine(value);
            }
            */
        }
    
    }
}
