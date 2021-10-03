using System;
using System.Threading.Tasks;
using ModelLibrary;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Write input path: ");
            //string imageFolder = Console.ReadLine();
            string imageFolder = @"D:\models\Assets\Images";
            await Detector.DetectImage(imageFolder);
        }
    }
}
