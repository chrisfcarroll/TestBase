using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TestBase
{
    public static class MvcViewResultShoulds
    {
        public static T ShouldBeViewWithModel<T>(this IActionResult @this, string viewName)
        {
            return @this.ShouldBeViewResultNamed(viewName).ViewData.Model.ShouldBeOfType<T>();
        }

        public static T ShouldBeViewWithModel<T>(this IActionResult @this)
        {
            return @this.ShouldBeViewResult().ViewData.Model.ShouldBeOfType<T>();
        }

        public static ViewResult ShouldBeViewNamed(this ViewResult @this, string viewName)
        {
            @this.ViewName.ToLower().ShouldEqual(viewName.ToLower());
            return @this;
        }

        public static ViewResult ShouldBeDefaultView(this IActionResult @this)
        {
            var thisView = @this.ShouldBeViewResult();
            Assert.That(thisView.ViewName == "" || thisView.ViewName.ToLower() == "index",
                        "expected default view name, got {0}",
                        thisView.ViewName);
            return thisView;
        }

        public static object ShouldHaveViewDataForKey(this ViewResult @this, string key)
        {
            Assert.That(@this.ViewData.ContainsKey(key),
                        string.Format("Keys present: {0}", string.Join(";", @this.ViewData.Keys.ToArray())));
            return @this.ViewData[key];
        }

        public static object ShouldHaveViewDataForKeys(this ViewResult @this, params string[] keys)
        {
            foreach (var key in keys) @this.ShouldHaveViewDataForKey(key);

            return @this;
        }

        public static T ShouldHaveModel<T>(this ViewResult @this) { return @this.ViewData.Model.ShouldBeOfType<T>(); }

        public static ViewResult ShouldBeAValidModel(this ViewResult @this)
        {
            @this.ViewData.ModelState.IsValid.ShouldBeTrue();
            return @this;
        }

        public static ViewResult ShouldBeAnInvalidModel(this ViewResult @this)
        {
            @this.ViewData.ModelState.IsValid.ShouldBeFalse();
            return @this;
        }

        public static ViewDataDictionary ShouldContainKey(this ViewDataDictionary @this, string key)
        {
            @this.ContainsKey(key).ShouldBeTrue();
            return @this;
        }
    }
}
