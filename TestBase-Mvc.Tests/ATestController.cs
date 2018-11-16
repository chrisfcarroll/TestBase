using System.Text;
using System.Web.Mvc;
using TestBase.Shoulds;

namespace TestBaseMvc.Tests
{
    public class ATestController : Controller
    {
        static string ViewName = "ViewName";
        
        public ATestController(IDependency dependency){}

        public ActionResult AView(string parameter, string other, string thing)
        {
            var model= new MyViewModel
            {
                YouPassedIn = parameter??"(null)",
                LinkToSelf = Url.Action("AView","ATest"),
                LinkToOther= Url.Action(thing,other)
            };
            return View(ViewName,model);
        }

        public ActionResult AFileResult(string someContent, string contentTypeToReturn, string downloadFileNametoUse)
        {
            return File(Encoding.UTF8.GetBytes(someContent), contentTypeToReturn, downloadFileNametoUse);
        }
    }

    public class MyViewModel
    {
        public string YouPassedIn { get; set; }
        public string LinkToSelf { get; set; }
        public string LinkToOther { get; set; }
    }

    public class IDependency{}
}