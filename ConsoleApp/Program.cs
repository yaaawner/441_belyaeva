using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ModelLibrary;

namespace ConsoleApp
{
    class Program
    {
        private static async Task Consumer()
        {
            string print;
            while (true)
            {
                print = await Detector.bufferBlock.ReceiveAsync();
                if (print == "end")
                    break;
                Console.WriteLine(print);
            }
            
            /*
            while (await Detector.bufferBlock.TryReceive(out string value))
            {
                Console.WriteLine(value);
            }
            */
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
