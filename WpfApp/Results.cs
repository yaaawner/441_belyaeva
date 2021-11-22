using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WpfApp
{
    /*
    abstract public class Type
    {
        public string Info { get; set; }

        public Type(string info)
        {
            Info = info;
        }
    }
    */

    public class Results : IEnumerable<string>
    {
        public string Info { get; set; }
        public List<string> images { get; set; }

        public Results(string newType, string newImage) 
        {
            Info = newType;
            images = new List<string>();
            images.Add(newImage);
        }

        public override string ToString()
        {
            return Info;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)images).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)images).GetEnumerator();
        }
    }

    /*
    public class ResultCollection
    {
        //Results res;
        public static ObservableCollection<Results> res = new ObservableCollection<Results>();
        
        public void Add (string type, string imageName)
        {
            //bool flag = true;
            foreach (Results r in res)
            {
                if (r.type == type)
                {
                    r.images.Add(imageName);
                    //flag = false;
                    return;
                }
            }
            res.Add(new Results(type, imageName));

            //res.IndexOf()
        }

    }*/


}
