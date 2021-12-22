using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;

namespace Server.Controllers
{
    public class Bag
    {
        public string Folder { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class ResultsController : Controller
    {
        private static string imageFolder = @"D:\models\Assets\Images";

        [HttpGet] 
        public string Get()
        {
            return "Test";
        }

        [HttpPut]
        public async Task PutAsync(Bag bg)
        {
            imageFolder = bg.Folder;
            Debug.WriteLine("===================== DEBUG ======================");
            Debug.WriteLine(bg.Folder);
            Debug.WriteLine(imageFolder);
            await Task.WhenAll(Detector.DetectImage(imageFolder), Sandbox.Consumer());
        }
        
        [HttpGet("types")]
        public IEnumerable<string> GetTypes()
        {
            var db = new ResultContext();
            return db.GetTypes();
        }

        [HttpGet("types/{type}")]
        public IEnumerable<byte[]> /*string*/ GetObjectsByType(string type)
        {
            var db = new ResultContext();
            return db.GetObjectsByType(type);
        }

        [HttpDelete("types/{type}")]
        public void DeleteType(string type)
        {
            var db = new ResultContext();
            db.DeleteType(type);
        }
    }
}
