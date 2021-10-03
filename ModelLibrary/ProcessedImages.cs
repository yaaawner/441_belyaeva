using System;
using System.Collections.Concurrent;

namespace ModelLibrary
{
    class ProcessedImages
    {
        ConcurrentBag<string> bag = new ConcurrentBag<string>();
        int countOfImages;

        public ProcessedImages (int count)
        {
            countOfImages = count;
        } 

        public void Add (string image)
        {
            bag.Add(image);
            Console.Clear();
            Console.WriteLine(((double)bag.Count / countOfImages * 100).ToString() + "%");
        }
    }
}
