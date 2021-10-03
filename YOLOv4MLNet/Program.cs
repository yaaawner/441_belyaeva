using System;
using System.Collections.Generic;
using System.Text;
//using Detector;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace YOLOv4MLNet
{
    class Program
    {
        static async Task Main()
        {
            string imageFolder = @"D:\models\Assets\Images";
            await Detector.DetectImage(imageFolder);

        }
    }
}
