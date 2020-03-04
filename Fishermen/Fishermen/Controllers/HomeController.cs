using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace Fishermen.Controllers
{
    public class HomeController : Controller
    {
        public IHostEnvironment HostingEnv { get; }

        public HomeController(IHostEnvironment env)
        {
            HostingEnv = env;
        }
        public IActionResult Index()
        {
            // return Json(Path.Combine(HostingEnv.ContentRootPath,"wwwroot", "index.html"));
            return new PhysicalFileResult(Path.Combine(HostingEnv.ContentRootPath, "wwwroot", "index.html"), new MediaTypeHeaderValue("text/html"));
        }

        public IActionResult CustomQuery()
        {
            return View();
        }
    }
}