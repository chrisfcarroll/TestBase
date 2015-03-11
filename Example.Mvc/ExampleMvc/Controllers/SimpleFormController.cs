using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestBase.ExampleMvc4.Models;

namespace TestBase.ExampleMvc4.Controllers
{
    public class SimpleFormController : Controller
    {
        public ActionResult Index(string name)
        {
            return View(new SimpleFormModel{Name=name});
        }
    }
}
