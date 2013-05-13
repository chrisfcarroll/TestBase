using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class MvcShoulds
    {
        public static RedirectToRouteResult ShouldBeRedirectToRouteResult(this ActionResult @this)
        {
            return @this.ShouldBeOfType<RedirectToRouteResult>();
        }

        public static RedirectToRouteResult ShouldBeRedirectToRouteResult(this ActionResult @this, string action, string controller)
        {
            var result = @this.ShouldBeOfType<RedirectToRouteResult>();
            result.ShouldHaveRouteValue("controller", controller);

            if (String.IsNullOrEmpty(action))
                @this.ShouldBeRedirectToDefaultActionAndController();
            else
                result.ShouldHaveRouteValue("action", action);

            return result;
        }

        public static RedirectToRouteResult ShouldBeRedirectToRouteResultWithController(this ActionResult @this, string controller)
        {
            var result = @this.ShouldBeOfType<RedirectToRouteResult>();
            result.ShouldHaveRouteValue("controller", controller);

            result.ShouldBeRedirectToDefaultAction();

            return result;
        }

        public static RedirectToRouteResult ShouldBeRedirectToDefaultActionAndController(this ActionResult @this)
        {
            var result = @this.ShouldBeOfType<RedirectToRouteResult>();

            if(result.RouteValues.ContainsKey("controller") )
            {
                result.RouteValues["controller"].ShouldBeNullOrEmptyOrWhitespace(
                        "Expected ActionResult to be Redirect to default action, but a controller, '{0}', was specified.",
                        result.RouteValues["controller"]);
            }
            result.ShouldBeRedirectToDefaultAction();

            return result;
        }

        public static RedirectToRouteResult ShouldBeRedirectToDefaultAction(this RedirectToRouteResult @this)
        {
            if (!@this.RouteValues.ContainsKey("action")) return @this;

            var action = @this.RouteValues["action"].ToString();

            if (!String.IsNullOrEmpty(action)) { ShouldHaveRouteValue(@this, "action", "index"); }

            return @this;
        }

        public static RedirectToRouteResult ShouldBeRedirectToActionResult(this ActionResult @this, string action)
        {
            var result = @this.ShouldBeOfType<RedirectToRouteResult>();

            result.RouteValues.ContainsKey("controller")
                  .ShouldBeFalse(String.Format("Controller redirect found '{0}' not none expected.",
                                               result.RouteValues["controller"]));

            result.ShouldBeRedirectToAction(action);

            return result;
        }

        public static RedirectToRouteResult ShouldBeRedirectToActionResult(this ActionResult @this, string action, string controller)
        {
            var result = @this.ShouldBeOfType<RedirectToRouteResult>();

            result.RouteValues.Keys.ShouldContain("controller");
            result.RouteValues["controller"].ToString().ToLower().ShouldEqual(controller.ToLower());
            result.ShouldBeRedirectToAction(action);
            return result;
        }

        public static ViewResult ShouldBeViewResult(this ActionResult @this)
        {
            return @this.ShouldBeOfType<ViewResult>();
        }

        public static JsonResult ShouldBeJsonResult(this ActionResult @this)
        {
            return @this.ShouldBeOfType<JsonResult>();
        }

        public static PartialViewResult ShouldBePartialViewResult(this ActionResult @this)
        {
            return @this.ShouldBeOfType<PartialViewResult>();
        }

        public static void ShouldBeRedirectToController(this RedirectToRouteResult @this, string controller)
        {
            ShouldHaveRouteValue(@this, "controller", controller);
        }

        public static void ShouldBeRedirectToAction(this RedirectToRouteResult @this, string action)
        {
            ShouldHaveRouteValue(@this, "action", action);
        }

        public static RedirectToRouteResult ShouldHaveRouteValue(this RedirectToRouteResult @this, string key, object value)
        {
            Assert.That(@this.RouteValues.ContainsKey(key),
                        String.Format("Key \"{0}\" not found in routevalues <{1}>. Expected key with value \"{2}\".",
                                        key,
                                        String.Join(",", @this.RouteValues.Keys.ToArray()),
                                        value
                                        ));

            @this.RouteValues[key].ShouldEqual(value);
            return @this;
        }

        public static RedirectToRouteResult ShouldHaveRouteValue(this RedirectToRouteResult @this, string key)
        {
            Assert.That(@this.RouteValues.ContainsKey(key),
                        String.Format("Key \"{0}\" not found in routevalues <{1}>.",
                                        key,
                                        String.Join(",", @this.RouteValues.Keys.ToArray())
                                        ));
            return @this;
        }

        public static RedirectToRouteResult ShouldHaveRouteValue(this RedirectToRouteResult @this, string key, string value, [Optional] string message, params object[] args)
        {
            Assert.That(@this.RouteValues.ContainsKey(key),
                        message??String.Format("Key \"{0}\" not found in routevalues <{1}>.",
                                        key,
                                        String.Join(",", @this.RouteValues.Keys.ToArray())
                                        ),
                        args);
            @this.RouteValues[key].ToString().ShouldEqualIgnoringCase(value);
            return @this;
        }

        public static RedirectToRouteResult ShouldHaveRouteValue<T>(this RedirectToRouteResult @this, string key, Expression<Func<T, bool>> predicate)
        {
            Assert.That(@this.RouteValues.ContainsKey(key),
                        String.Format("Key \"{0}\" expected but not found in routevalues <{1}>.",
                                        key,
                                        String.Join(",", @this.RouteValues.Keys.ToArray())
                                        ));

            predicate.Compile()((T)@this.RouteValues[key])
                .ShouldBeTrue(String.Format("Expected {0}( RouteValues[{1}] ) to be true", predicate.Body, key));
            return @this;
        }

        public static RedirectToRouteResult ShouldHaveRouteIdValue(this RedirectToRouteResult @this, object id)
        {
            @this.ShouldHaveRouteValue("id", id);
            return @this;
        }

        public static ViewResultBase ShouldBeViewNamed(this ViewResultBase @this, string viewName)
        {
            @this.ViewName.ToLower().ShouldEqual(viewName.ToLower());
            return @this;
        }

        public static ViewResult ShouldBeDefaultView(this ActionResult @this)
        {
            var @thisView = @this.ShouldBeViewResult();
            Assert.That(thisView.ViewName == "" || thisView.ViewName.ToLower() == "index", "expected default view name, got {0}", thisView.ViewName);
            return thisView;
        }

        public static object ShouldHaveViewDataForKey(this ViewResultBase @this, string key)
        {
            Assert.That(@this.ViewData.ContainsKey(key), Is.True,
                        String.Format("Keys present: {0}",
                                      String.Join(";", @this.ViewData.Keys.ToArray())));
            return @this.ViewData[key];
        }

        public static object ShouldHaveViewDataForKeys(this ViewResultBase @this, params string[] keys)
        {
            foreach (var key in keys)
            {
                @this.ShouldHaveViewDataForKey(key);
            }
            return @this;
        }

        public static T ShouldHaveModel<T>(this ViewResultBase @this)
        {
            return @this.ViewData.Model.ShouldBeOfType<T>();
        }

        public static ViewResultBase ShouldBeAValidModel(this ViewResultBase @this)
        {
            @this.ViewData.ModelState.IsValid.ShouldBeTrue();
            return @this;
        }

        public static ViewResultBase ShouldBeAnInvalidModel(this ViewResultBase @this)
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
