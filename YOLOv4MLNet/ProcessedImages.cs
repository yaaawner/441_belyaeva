using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace YOLOv4MLNet
{
    class ProcessedImages
    {
        ConcurrentBag<string> bag = new ConcurrentBag<string>();
        int countOfImages;

        public ProcessedImages (int count)
        {
            countOfImages = count;
        } 

        public void AddToBag (string image)
        {
            bag.Add(image);
            Console.Clear();
            //Console.WriteLine(bag.Count.ToString());
            Console.WriteLine(((double)bag.Count / countOfImages * 100).ToString() + "%");
        }
    }
}
