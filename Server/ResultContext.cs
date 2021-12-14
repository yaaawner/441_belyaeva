using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Collections;
using ModelLibrary;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Server
{
    public class ResultsView
    {

    }

    public class DetectedObject
    {
        public int DetectedObjectId { get; set; }
        public string Path { get; set; }
        public float x1 { get; set; }
        public float y1 { get; set; }
        public float x2 { get; set; }
        public float y2 { get; set; }
        public byte[] BitmapImage { get; set; }
        public string OutputPath { get; set; }
        public Results Type { get; set; }
        public override string ToString()
        {
            return OutputPath;
        }
    }

    public class Results// : IEnumerable<string>
    {
        public int ResultsId { get; set; }
        public string Type { get; set; }
        public ICollection<DetectedObject> DetectedObjects { get; set; }

        public override string ToString()
        {
            return Type;
        }
    }

    
    public class ResultContext : DbContext
    {
        //public UserContext() : base("DbConnection") { }
        public DbSet<Results> Results { get; set; }
        public DbSet<DetectedObject> DetectedObject { get; set; }
        //public DbSet<DetectedObjectDetails> DetectedObjectDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder o)
           => o.UseSqlite(@"Data Source=D:\MLResults.db");

        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public void AddElem (string type, string path, float[] BBox, Bitmap bitmap)
        {
            var dobj = new DetectedObject();

            dobj.Type = new Results();
            var query = Results.Where(p => type == p.Type);
            if (query.Count() > 0)
            {
                dobj.Type = query.First();
            } 
            else
            {
                Results results = new Results();
                dobj.Type = results;
                dobj.Type.Type = type;
                Results.Add(dobj.Type);
            }
            dobj.x1 = BBox[0];
            dobj.y1 = BBox[1];
            dobj.x2 = BBox[2];
            dobj.y2 = BBox[3];
            dobj.BitmapImage = ImageToByte2(bitmap);
            dobj.Path = path;
            dobj.OutputPath = path;
            DetectedObject.Add(dobj);
            SaveChanges();
        }

        public IEnumerable<string> GetTypes()
        {
            foreach (var res in Results)
            {
                yield return res.Type;
            }
        }

        public IEnumerable<string> GetObjectsByType(string type)
        {
            foreach (var obj in DetectedObject.Where(p => p.Type.Type == type))
            {
                yield return obj.Path;
            }
        }

        public void Clear()
        {
            foreach (var dobj in DetectedObject)
            {
                DetectedObject.Remove(dobj);
            }
            foreach (var res in Results)
            {
                Results.Remove(res);
            }
            SaveChanges();
        }
        

        public void DeleteType(string type)
        {
            foreach (var dobj in DetectedObject)
            {
                if (dobj.Type.Type == type)
                {
                    DetectedObject.Remove(dobj);
                }
            }
            foreach (var res in Results)
            {
                if (res.Type == type)
                {
                    Results.Remove(res);
                }
            }

            SaveChanges();
        } 

        /*
        public IEnumerable<string> GetObjects (string type)
        {
            //var query = Results.Where()
            foreach (var res in Results)
            {
                if (res.Type == type)
                {
                    foreach (var dobj in res.DetectedObjects)
                    {
                        yield return dobj.OutputPath;
                    }
                    break;
                }
            }
        }
        */
        



        //public IEnumerable<>


    }

    public class Sandbox
    {
        //public static ResultContext db = new ResultContext();
        //public static ObservableCollection<Results> resultCollection = new ObservableCollection<Results>();
        //public static ObservableCollection<Results> resultCollection = db.Results.Local.ToObservableCollection();

        public static async Task Consumer()
        {
            var db = new ResultContext();
            
                while (true)
                {
                    string type;
                    string image;
                    Bitmap bitmap;
                    float[] BBox;

                    (type, image, bitmap, BBox) = await Detector.resultBufferBlock.ReceiveAsync();
                    if (type == "end")
                    {
                        db.SaveChanges();
                        break;
                    }
                    else
                    {
                        db.AddElem(type, image, BBox, bitmap);
                    }

                }
            
        }
    }

}
