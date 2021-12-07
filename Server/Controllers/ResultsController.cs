using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResultsController : Controller
    {
        string imageFolder = @"D:\models\Assets\Images";
        [HttpGet] 
        public string Get()
        {
            return "Test";
        }

        [HttpGet("task")]
        public async Task GetTypesAsync()
        {
            await Task.WhenAll(Detector.DetectImage(imageFolder), Sandbox.Consumer());
        }

        [HttpGet("types")]
        public IEnumerable<string> GetTypes()
        {

            var db = new ResultContext();
            
                return db.GetTypes();
            

        }

        [HttpGet("{type}")]
        public IEnumerable<string> GetObjects(string type)
        {
            using (var db = new ResultContext())
            {
                return db.GetObjects(type);
            }
        }
        

    }

}
