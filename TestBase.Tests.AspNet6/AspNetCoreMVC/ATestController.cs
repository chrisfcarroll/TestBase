using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestBase.Tests.AspNet6.AspNetCoreMVC
{
    public class ATestController : Controller
    {
        static readonly string ViewName = "ViewName";

        public IActionResult AView(string parameter, string other, string thing)
        {
            var model = new MyViewModel
                        {
                        YouPassedIn = parameter ?? "(null)",
                        LinkToSelf  = Url.Action("AView", "ATest"),
                        LinkToOther = Url.Action(thing,   other)
                        };
            return View(ViewName, model);
        }

        public IActionResult AFileResult(string someContent, string contentTypeToReturn, string downloadFileNametoUse)
        {
            return File(Encoding.UTF8.GetBytes(someContent), contentTypeToReturn, downloadFileNametoUse);
        }

        public string SomethingWithCookies(string cookie1, string allCookiesNewValue, string newCookie)
        {
            var from = Request.Cookies[cookie1];
            Response.Cookies.Append(cookie1,   allCookiesNewValue, new CookieOptions {Path = "/A"});
            Response.Cookies.Append(newCookie, allCookiesNewValue);
            return from;
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
