﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class MvcViewResultShoulds
    {
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
