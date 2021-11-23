using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ModelLibrary;
using System.Collections.Generic;
using System.Data.Entity;

namespace ConsoleApp
{
    public class DetectedObject
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
    }

    class UserContext : DbContext
    {
        public UserContext() : base("DbConnection") { }

        public DbSet<DetectedObject> DetectedObjects { get; set; }
    }

    class Program
    {
        private static async Task Consumer()
        {
            string type;
            string image;

            using (UserContext db = new UserContext())
            {
                while (true)
                {
                    (type, image) = await Detector.resultBufferBlock.ReceiveAsync();
                    if (type == "end")
                    {
                        db.SaveChanges();
                        break;
                    }
                    DetectedObject result = new DetectedObject { Type = type, Path = image };
                    db.DetectedObjects.Add(result);
                }

                var detectedObjects = db.DetectedObjects;
                Console.WriteLine("Список объектов:");
                foreach (DetectedObject dobj in detectedObjects)
                {
                    Console.WriteLine("{0} - {1}", dobj.Type, dobj.Path);
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
