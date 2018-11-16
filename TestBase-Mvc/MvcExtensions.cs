using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using TestBase.Shoulds;

namespace TestBase
{
    public static class MvcExtensions
    {
        public static bool IsRedirectToRoute(this ActionResult actionResult)
        {
            return actionResult is RedirectToRouteResult;
        }

        public static string RedirectController(this ActionResult actionResult)
        {
            return ((RedirectToRouteResult) actionResult).RouteValues["controller"].ToString();
        }

        public static string RedirectAction(this ActionResult actionResult)
        {
            return ((RedirectToRouteResult) actionResult).RouteValues["action"].ToString();
        }

        public static bool IsView(this ActionResult actionResult)
        {
            return actionResult is ViewResult;
        }

        public static bool ViewNameIs(this ActionResult actionResult, string expectedViewName)
        {
            return ((ViewResult) actionResult).ViewName.Equals(expectedViewName);
        }

        public static TController WithModelStateIsInvalid<TController>(this TController @this)
            where TController : Controller
        {
            @this.ModelState.AddModelError("SomeKey", @"Some error message");

            return @this;
        }

        public static TController WithValidationforModel<TController, TModel>(this TController @this, TModel model,
            IValueProvider valueProvider = null)
            where TController : Controller
        {
            var modelBinder = new ModelBindingContext()
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(TModel)),
                ValueProvider = valueProvider ?? new NameValueCollectionValueProvider(
                                    new NameValueCollection(),
                                    CultureInfo.InvariantCulture)
            };
            new DefaultModelBinder().BindModel(new ControllerContext(), modelBinder);

            @this.ModelState.Clear();
            @this.ModelState.Merge(modelBinder.ModelState);
            return @this;
        }

        public static string ViewFile(this Controller controller, ViewResultBase viewResult, string namespacePathToMvcRoot)
        {
            return ViewFile(controller, viewResult.ViewName + ".aspx", namespacePathToMvcRoot);
        }

        public static string ViewFile<TC>(this TC controller, string viewFileName, string namespacePathToMvcRoot) where TC : Controller
        {
            var pathToViewFile = "..\\..\\..\\" +
                                 controller.GetType().Namespace
                                     .Replace(namespacePathToMvcRoot, "")
                                     .Replace(".", "\\")
                                     .Replace("\\Controllers", "\\Views\\") +
                                 controller.GetType().Name.Replace("Controller", "\\");

            var fi = new FileInfo(pathToViewFile + viewFileName);
            fi.Exists
                .ShouldBeTrue(String.Format("Couldn't find viewfile {0} for controller {1}, view {2}",
                    fi, controller.GetType().Name, viewFileName));

            using (var viewFile = new StreamReader(fi.FullName))
            {
                return viewFile.ReadToEnd();
            }
        }
    }
}