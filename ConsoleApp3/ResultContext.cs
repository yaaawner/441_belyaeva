using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using System.Drawing;

namespace ConsoleApp3
{
    public class DetectedObject
    {
        public int DetectedObjectId { get; set; }
        public string Path { get; set; }
        public float x1 { get; set; }
        public float y1 { get; set; }
        public float x2 { get; set; }
        public float y2 { get; set; }
        public byte[] BitmapImage { get; set; }
        public Results Type { get; set; }
    }

    public class Results
    {
        public int ResultsId { get; set; }
        public string Type { get; set; }
        public ICollection<DetectedObject> DetectedObjects { get; set; }
    }
    class ResultContext : DbContext
    {
        //public UserContext() : base("DbConnection") { }
        public DbSet<Results> Results { get; set; }
        public DbSet<DetectedObject> DetectedObject { get; set; }
        //public DbSet<DetectedObjectDetails> DetectedObjectDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder o)
           => o.UseSqlite(@"Data Source=MLResults.db");

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
                dobj.Type = new Results();
                dobj.Type.Type = type;
                Results.Add(dobj.Type);
            }
            dobj.x1 = BBox[0];
            dobj.y1 = BBox[1];
            dobj.x2 = BBox[2];
            dobj.y2 = BBox[3];
            dobj.BitmapImage = ImageToByte2(bitmap);
            DetectedObject.Add(dobj);
            SaveChanges();

            /*
            bool flag = true;
            foreach (var res in Results)
            {
                if (res.Type == type)
                {
                    bool flagimg = true;
                    foreach (var obj in res.DetectedObjects)
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
                            Path = path,
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
                    Path = path,
                    x1 = BBox[0],
                    y1 = BBox[1],
                    x2 = BBox[2],
                    y2 = BBox[3],
                    BitmapImage = ImageToByte2(bitmap)
                };
                result.DetectedObjects.Add(objects);
                Results.Add(result);
                //db.Results

            }

            SaveChanges();
            */
        }

        public IEnumerable<string> GetTypes()
        {
            foreach (var res in Results)
            {
                yield return res.Type;
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



        //public IEnumerable<>
    }
}
