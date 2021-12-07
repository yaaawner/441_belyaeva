using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WpfApp
{
    
    public class ResultsView : IEnumerable<string>
    {
        public string Type { get; set; }
        public List<string> images { get; set; }

        public ResultsView(string newType, string newImage) 
        {
            Type = newType;
            images = new List<string>();
            images.Add(newImage);
        }

        public override string ToString()
        {
            return Type;
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
    
}
