﻿using System.Diagnostics;
using GuineaPig.AspNetCore2.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuineaPig.AspNetCore2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() { return View(); }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
