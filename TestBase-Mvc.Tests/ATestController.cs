using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TestBaseMvc.Tests
{
    public class StubController : Controller
    {
    }

    public class ATestController : Controller
    {
        const string ViewName = "ViewName";

        public ATestController(IDependency dependency) { }

        public ActionResult AView(string parameter, string other, string thing)
        {
            var model = new MyViewModel
                        {
                        YouPassedIn = parameter ?? "(null)",
                        LinkToSelf  = Url.Action("AView", "ATest"),
                        LinkToOther = Url.Action(thing,   other)
                        };
            return View(ViewName, model);
        }

        public ActionResult AFileResult(string someContent, string contentTypeToReturn, string downloadFileNametoUse)
        {
            return File(Encoding.UTF8.GetBytes(someContent), contentTypeToReturn, downloadFileNametoUse);
        }

        public string SomethingWithCookies(string cookie1, string cookie2, string newValue)
        {
            var was = Request.Cookies[cookie1];
            Response.Cookies[cookie1].Value = newValue;
            Response.Cookies.Add(new HttpCookie(cookie2, newValue));

            return was.Value;
        }
    }

    public class MyViewModel
    {
        public string YouPassedIn { get; set; }
        public string LinkToSelf  { get; set; }
        public string LinkToOther { get; set; }
    }

    public class IDependency
    {
    }
}
