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
    /*
    [Keyless]
    public class DetectedObjectDetails
    {
        public int DetectedObjectDetailsId { get; set; }
        public byte[] BitmapImage { get; set; }
    }
    */

    public class DetectedObject
    {
        public int DetectedObjectId { get; set; }
        public string Path { get; set; }
        public float x1 { get; set; }
        public float y1 { get; set; }
        public float x2 { get; set; }
        public float y2 { get; set; }
        //public byte[] BitmapImage { get; set; }
        //public int DetectedObjectDetailsId { get; set; }
        //virtual public DetectedObjectDetails Details { get; set; }
        public byte[] BitmapImage { get; set; }

    }

    public class Results
    {
        public int ResultsId { get; set; }
        public string Type { get; set; }
        public ICollection<DetectedObject> DetectedObjects { get; set; }
    }

    class UserContext : DbContext
    {
        public DbSet<Results> Results { get; set; }
        public DbSet<DetectedObject> DetectedObject { get; set; }
        //public DbSet<DetectedObjectDetails> DetectedObjectDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder o)
           => o.UseSqlite("Data Source=MLResults.db");
    }
    class Program
    {
        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        private static async Task Consumer()
        {
            string type;
            string image;
            Bitmap bitmap;
            float[] BBox;

            using (UserContext db = new UserContext())
            {
                while (true)
                {
                    (type, image, bitmap, BBox) = await Detector.resultBufferBlock.ReceiveAsync();
                    if (type == "end")
                    {
                        db.SaveChanges();
                        break;
                    }
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
                    //DetectedObjectDetails details = new DetectedObjectDetails { BitmapImage = ImageToByte2(bitmap) };
                    //objects.Details = details;
                    //db.DetectedObjectDetails.Add(details);
                    //db.DetectedObject.Add(objects);
                    db.Results.Add(result);
                    
                    db.SaveChanges();
                }
                foreach (var res in db.Results.Include(a => a.DetectedObjects))
                {
                    Console.WriteLine($"{res.ResultsId} {res.Type}");
                    // db.Entry(a2).Collection(a => a.Books).Load();
                    foreach (var d in res.DetectedObjects)
                        Console.WriteLine($"       {d.DetectedObjectId} {d.Path}");
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
