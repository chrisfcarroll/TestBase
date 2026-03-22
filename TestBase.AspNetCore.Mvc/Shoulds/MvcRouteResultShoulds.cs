using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace TestBase
{
    public static class MvcRouteResultShoulds
    {
        public static RedirectToRouteResult ShouldHaveRouteValue(
            this RedirectToRouteResult @this,
            string                     key,
            object                     value)
        {
            Assert.That(@this.RouteValues.ContainsKey(key),
                        string.Format("Key \"{0}\" not found in routevalues <{1}>. Expected key with value \"{2}\".",
                                      key,
                                      string.Join(",", @this.RouteValues.Keys.ToArray()),
                                      value
                                     ));

            @this.RouteValues[key].ShouldEqual(value);
            return @this;
        }

        public static RedirectToRouteResult ShouldHaveRouteValue(this RedirectToRouteResult @this, string key)
        {
            Assert.That(@this.RouteValues.ContainsKey(key),
                        string.Format("Key \"{0}\" not found in routevalues <{1}>.",
                                      key,
                                      string.Join(",", @this.RouteValues.Keys.ToArray())
                                     ));
            return @this;
        }

        public static RedirectToRouteResult ShouldHaveRouteValue(
            this RedirectToRouteResult @this,
            string                     key,
            string                     value,
            [Optional] string          message,
            params     object[]        args)
        {
            Assert.That(@this.RouteValues.ContainsKey(key),
                        message
                     ?? string.Format("Key \"{0}\" not found in routevalues <{1}>.",
                                      key,
                                      string.Join(",", @this.RouteValues.Keys.ToArray())
                                     ),
                        args);
            @this.RouteValues[key].ToString().ShouldEqualIgnoringCase(value);
            return @this;
        }

        public static RedirectToRouteResult ShouldHaveRouteValue<T>(
            this RedirectToRouteResult @this,
            string                     key,
            Expression<Func<T, bool>>  predicate)
        {
            Assert.That(@this.RouteValues.ContainsKey(key),
                        string.Format("Key \"{0}\" expected but not found in routevalues <{1}>.",
                                      key,
                                      string.Join(",", @this.RouteValues.Keys.ToArray())
                                     ));

            predicate.Compile()((T) @this.RouteValues[key])
                     .ShouldBeTrue(string.Format("Expected {0}( RouteValues[{1}] ) to be true", predicate.Body, key));
            return @this;
        }

        public static RedirectToRouteResult ShouldHaveRouteIdValue(this RedirectToRouteResult @this, object id)
        {
            @this.ShouldHaveRouteValue("id", id);
            return @this;
        }
    }
}
