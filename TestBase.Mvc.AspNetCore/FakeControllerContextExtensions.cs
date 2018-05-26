#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders.Testing;
using Moq;

namespace TestBase
{
    public static class StubbedAspNetCoreControllerContextExtensions
    {
        public static T WithRequestHeader<T>(this T controller, string name, params string[] values) where T : Controller
        {
            controller.ControllerContext.HttpContext.Request.Headers.Add(name, values);
            return controller;
        }

        public static T WithControllerContext<T>(this T controller,
                                                 string action = "action",
                                                 object routeValues = null,
                                                 string virtualPathTemplate = "/{controller}/{action}",
                                                 ClaimsPrincipal user = null) where T : Controller
        {
            var controllerName = controller.GetType().Name;
            if (controllerName.EndsWith("Controller") && controllerName.Length > 10)
            {
                controllerName = controllerName.Substring(0, controllerName.Length - 10);
            }

            var actionDescriptor = new ActionDescriptor
                                   {
                                       RouteValues = new Dictionary<string, string>(new RouteValueEqualityComparer())
                                                     {
                                                         {"controller", controllerName},
                                                         {"action", action}
                                                     }
                                   };
            foreach (var kv in new RouteValueDictionary(routeValues ?? new { })) { actionDescriptor.RouteValues.Add(kv.Key, kv.Value.ToString()); }

            //---Doesnt seem to help
            //var trb = new TreeRouteBuilder(
            //    new LoggerFactory().AddConsole(), 
            //    new UrlTestEncoder(),
            //    new DefaultObjectPool<UriBuildingContext>( new TestPooledObjectPolicy<UriBuildingContext>(){Factory = ()=>new TestUriBuildingContext()}),
            //    new DefaultInlineConstraintResolver(
            //        new OptionsManager<RouteOptions>(
            //            new OptionsFactory<RouteOptions>(
            //                new IConfigureOptions<RouteOptions>[0],
            //                new IPostConfigureOptions<RouteOptions>[0]))));
            //var router = trb.Build();
            //--- 
            var routerMock = new Mock<IRouter>();
            routerMock
                .Setup(x => x.GetVirtualPath(It.IsAny<VirtualPathContext>()))
                .Returns<VirtualPathContext>(vpc =>
                                                 new VirtualPathData(routerMock.Object, VirtualPathFromTemplateAndValues(virtualPathTemplate, vpc.Values), vpc.Values)
                                            );
            var routeData = new RouteData();
            routeData.Routers.Add(routerMock.Object);
            var metadataProvider = TestModelMetadataProvider.CreateDefaultProvider();
            var httpContext = new DefaultHttpContext {User = user ?? new ClaimsPrincipal(new ClaimsIdentity(new Claim[0]))};

            var actionContext = new ActionContext(
                                                  httpContext,
                                                  routeData,
                                                  actionDescriptor,
                                                  new ModelStateDictionary());

            var viewData = new ViewDataDictionary(metadataProvider, new ModelStateDictionary());
            var tempData = new TempDataDictionary(httpContext, new SessionStateTempDataProvider());
            controller.MetadataProvider = metadataProvider;
            controller.ViewData = viewData;
            controller.TempData = tempData;
            controller.ObjectValidator = new DefaultObjectValidator(metadataProvider, new List<IModelValidatorProvider>());
            controller.Url = new UrlHelper(actionContext);
            controller.ControllerContext = new ControllerContext {HttpContext = httpContext};
            return controller;
        }

        static string VirtualPathFromTemplateAndValues(string virtualPathTemplate, RouteValueDictionary routeValues)
        {
            var fakeVirtualPath = virtualPathTemplate;
            var valuesInTemplate = new List<KeyValuePair<string, object>>();
            foreach (var kv in routeValues)
                if (fakeVirtualPath.IndexOf("{" + kv.Key + "}", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    fakeVirtualPath = Regex.Replace(fakeVirtualPath, "{" + kv.Key + "}", kv.Value.ToString(), RegexOptions.IgnoreCase);
                    valuesInTemplate.Add(kv);
                }

            var otherValues = routeValues.Except(valuesInTemplate);
            var parms = string.Join("&", otherValues.Select(kv => $"{kv.Key}={kv.Value}"));
            if (!string.IsNullOrEmpty(parms)) fakeVirtualPath += "?" + parms;
            return fakeVirtualPath;
        }

        /// <summary>
        /// Probably  not needed, as the real <see cref="UrlHelper"/> mostly works in unit tests given a fake <see cref="ActionContext"/>
        /// 
        /// Adds a Mock<see cref="UrlHelper"/> which returns
        /// Action routes of the form "{uac.Controller}/{uac.Action}#{uac.Fragment}?{uac.Values.Key[i]}={uac.Values.Value[i]}..."
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <returns></returns>
        static T WithMockUrlHelper<T>(this T controller) where T : Controller
        {
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns((UrlActionContext uac) =>
                             $"{uac.Controller}/{uac.Action}#{uac.Fragment}?"
                             + string.Join("&", new RouteValueDictionary(uac.Values).Select(p => p.Key + "=" + p.Value)));
            urlHelperMock
                .Setup(x => x.Content(It.IsAny<string>()))
                .Returns<string>(s => s);
            controller.Url = urlHelperMock.Object;
            return controller;
        }
    }


    public class TestOptionsManager<TOptions> : IOptions<TOptions> where TOptions : class, new()
    {
        public TestOptionsManager() : this(new TOptions())
        {
        }

        public TestOptionsManager(TOptions value)
        {
            Value = value;
        }

        public TOptions Value { get; }
    }

    public class TestModelMetadataProvider : DefaultModelMetadataProvider
    {
        // Creates a provider with all the defaults - includes data annotations
        public static IModelMetadataProvider CreateDefaultProvider(IStringLocalizerFactory stringLocalizerFactory = null)
        {
            var detailsProviders = new IMetadataDetailsProvider[]
                                   {
                                       new DefaultBindingMetadataProvider(),
                                       new DefaultValidationMetadataProvider(),
                                       new DataAnnotationsMetadataProvider(new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(), stringLocalizerFactory),
                                       //new DataMemberRequiredBindingMetadataProvider(),
                                   };

            var compositeDetailsProvider = new DefaultCompositeMetadataDetailsProvider(detailsProviders);
            return new DefaultModelMetadataProvider(compositeDetailsProvider, new TestOptionsManager<MvcOptions>());
        }

        public static IModelMetadataProvider CreateDefaultProvider(IList<IMetadataDetailsProvider> providers)
        {
            var detailsProviders = new List<IMetadataDetailsProvider>()
                                   {
                                       new DefaultBindingMetadataProvider(),
                                       new DefaultValidationMetadataProvider(),
                                       new DataAnnotationsMetadataProvider(new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(), stringLocalizerFactory: null),
                                       //new DataMemberRequiredBindingMetadataProvider(),
                                   };

            detailsProviders.AddRange(providers);

            var compositeDetailsProvider = new DefaultCompositeMetadataDetailsProvider(detailsProviders);
            return new DefaultModelMetadataProvider(compositeDetailsProvider, new TestOptionsManager<MvcOptions>());
        }

        public static IModelMetadataProvider CreateProvider(IList<IMetadataDetailsProvider> providers)
        {
            var detailsProviders = new List<IMetadataDetailsProvider>();
            if (providers != null)
            {
                detailsProviders.AddRange(providers);
            }

            var compositeDetailsProvider = new DefaultCompositeMetadataDetailsProvider(detailsProviders);
            return new DefaultModelMetadataProvider(compositeDetailsProvider, new TestOptionsManager<MvcOptions>());
        }

        readonly TestModelMetadataDetailsProvider _detailsProvider;

        public TestModelMetadataProvider()
            : this(new TestModelMetadataDetailsProvider())
        {
        }

        TestModelMetadataProvider(TestModelMetadataDetailsProvider detailsProvider)
            : base(
                   new DefaultCompositeMetadataDetailsProvider(new IMetadataDetailsProvider[]
                                                               {
                                                                   new DefaultBindingMetadataProvider(),
                                                                   new DefaultValidationMetadataProvider(),
                                                                   new DataAnnotationsMetadataProvider(new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(),
                                                                                                       stringLocalizerFactory: null),
                                                                   detailsProvider
                                                               }),
                   new TestOptionsManager<MvcOptions>())
        {
            _detailsProvider = detailsProvider;
        }

        public IMetadataBuilder ForType(Type type)
        {
            var key = ModelMetadataIdentity.ForType(type);

            var builder = new MetadataBuilder(key);
            _detailsProvider.Builders.Add(builder);
            return builder;
        }

        public IMetadataBuilder ForType<TModel>()
        {
            return ForType(typeof(TModel));
        }

        public IMetadataBuilder ForProperty(Type containerType, string propertyName)
        {
            var property = containerType.GetRuntimeProperty(propertyName).ShouldNotBeNull();

            var key = ModelMetadataIdentity.ForProperty(property.PropertyType, propertyName, containerType);

            var builder = new MetadataBuilder(key);
            _detailsProvider.Builders.Add(builder);
            return builder;
        }

        public IMetadataBuilder ForProperty<TContainer>(string propertyName)
        {
            return ForProperty(typeof(TContainer), propertyName);
        }

        class TestModelMetadataDetailsProvider :
            IBindingMetadataProvider,
            IDisplayMetadataProvider,
            IValidationMetadataProvider
        {
            public List<MetadataBuilder> Builders { get; } = new List<MetadataBuilder>();

            public void CreateBindingMetadata(BindingMetadataProviderContext context)
            {
                foreach (var builder in Builders)
                {
                    builder.Apply(context);
                }
            }

            public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
            {
                foreach (var builder in Builders)
                {
                    builder.Apply(context);
                }
            }

            public void CreateValidationMetadata(ValidationMetadataProviderContext context)
            {
                foreach (var builder in Builders)
                {
                    builder.Apply(context);
                }
            }
        }

        public interface IMetadataBuilder
        {
            IMetadataBuilder BindingDetails(Action<BindingMetadata> action);

            IMetadataBuilder DisplayDetails(Action<DisplayMetadata> action);

            IMetadataBuilder ValidationDetails(Action<ValidationMetadata> action);
        }

        class MetadataBuilder : IMetadataBuilder
        {
            List<Action<BindingMetadata>> _bindingActions = new List<Action<BindingMetadata>>();
            List<Action<DisplayMetadata>> _displayActions = new List<Action<DisplayMetadata>>();
            List<Action<ValidationMetadata>> _valiationActions = new List<Action<ValidationMetadata>>();

            readonly ModelMetadataIdentity _key;

            public MetadataBuilder(ModelMetadataIdentity key)
            {
                _key = key;
            }

            public void Apply(BindingMetadataProviderContext context)
            {
                if (_key.Equals(context.Key))
                {
                    foreach (var action in _bindingActions)
                    {
                        action(context.BindingMetadata);
                    }
                }
            }

            public void Apply(DisplayMetadataProviderContext context)
            {
                if (_key.Equals(context.Key))
                {
                    foreach (var action in _displayActions)
                    {
                        action(context.DisplayMetadata);
                    }
                }
            }

            public void Apply(ValidationMetadataProviderContext context)
            {
                if (_key.Equals(context.Key))
                {
                    foreach (var action in _valiationActions)
                    {
                        action(context.ValidationMetadata);
                    }
                }
            }

            public IMetadataBuilder BindingDetails(Action<BindingMetadata> action)
            {
                _bindingActions.Add(action);
                return this;
            }

            public IMetadataBuilder DisplayDetails(Action<DisplayMetadata> action)
            {
                _displayActions.Add(action);
                return this;
            }

            public IMetadataBuilder ValidationDetails(Action<ValidationMetadata> action)
            {
                _valiationActions.Add(action);
                return this;
            }
        }
    }

    public class FakeUrlHelper : IUrlHelper
    {
        public FakeUrlHelper(ActionContext actionContext)
        {
            ActionContext = actionContext;
        }

        public string Action(UrlActionContext actionContext)
        {
            return actionContext.Action;
        }

        public string Content(string contentPath)
        {
            return contentPath;
        }

        public bool IsLocalUrl(string url) => true;

        public string RouteUrl(UrlRouteContext routeContext)
        {
            return $"{routeContext.Protocol}://{routeContext.Host}/{routeContext.RouteName}";
        }

        public string Link(string routeName, object values)
        {
            return routeName + "?" + String.Join("&", new RouteValueDictionary(values).Select(p => p.Key + "=" + p.Value));
        }

        public ActionContext ActionContext { get; }
    }

    public class TestUriBuildingContext : UriBuildingContext
    {
        public TestUriBuildingContext() : base(new UrlTestEncoder())
        {
        }
    }

    public class TestPooledObjectPolicy<T> : IPooledObjectPolicy<T> where T : class
    {
        public Func<T> Factory = () => throw new InvalidOperationException("First, populate TestPooledObjectPolicy<T>.Factory, then you can use it.");

        public T Create()
        {
            return Factory();
        }

        public bool Return(T obj)
        {
            return true;
        }
    }
}
#endif