using System.Threading.Tasks;
using ModelLibrary;

namespace ConsoleApp
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
