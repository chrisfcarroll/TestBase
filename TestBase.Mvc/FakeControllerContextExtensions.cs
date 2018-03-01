#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace TestBase
{
    public static class FakeControllerContextExtensions
    {
        public static T WithRequestHeader<T>(this T controller, string name, params string[] values) where T : Controller
        {
            controller.ControllerContext.HttpContext.Request.Headers.Add(name, values);
            return controller;
        }


        public static T WithControllerContext<T>(this T controller, string action, ClaimsPrincipal user) where T : Controller
        {
            var metadataProvider = TestModelMetadataProvider.CreateDefaultProvider();
            var httpContext = new DefaultHttpContext();
            var urlActionContext = ActionContextFor<T>(action);
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var viewData = new ViewDataDictionary(metadataProvider, new ModelStateDictionary());
            var tempData = new TempDataDictionary( httpContext, new SessionStateTempDataProvider());
            controller.MetadataProvider = metadataProvider;
            controller.ViewData = viewData;
            controller.TempData = tempData;
            controller.ObjectValidator = new DefaultObjectValidator( metadataProvider,new List<IModelValidatorProvider>());
            controller.ControllerContext = ControllerContextWith(user);
            WithUrlHelper(controller, actionContext);
            return controller;
        }

        static UrlActionContext ActionContextFor<T>(string action) where T : Controller
        {
            return new UrlActionContext
            {
                Controller = typeof(T).Name,
                Action = action,
                Fragment = "",
                Host = "localhost",
                Protocol = "http",
                Values= new {}
            };
        }

        static T WithUrlHelper<T>(this T controller, ActionContext actionContext) where T : Controller
        {
            controller.Url = new Microsoft.AspNetCore.Mvc.Routing.UrlHelper(actionContext);
            //var urlHelperMock = new Mock<IUrlHelper>();
            //urlHelperMock
            //    .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            //    .Returns((UrlActionContext uac) =>
            //                 $"{uac.Controller}/{uac.Action}#{uac.Fragment}?"
            //                 + string.Join("&", new RouteValueDictionary(uac.Values).Select(p => p.Key + "=" + p.Value)));
            //controller.Url = urlHelperMock.Object;
            return controller;
        }

        public static T WithControllerContext<T>(this T controller, string action) where T : Controller
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[0]));
            return WithControllerContext(controller, action, user);
        }


        public static ControllerContext ControllerContextWith(ClaimsPrincipal claimsPrincipal)
        {
            return new ControllerContext()
                   {
                       HttpContext = new DefaultHttpContext
                                     {
                                         User = claimsPrincipal,
                                     }
                   };
        }
    }


    public class TestOptionsManager<TOptions> : IOptions<TOptions> where TOptions : class, new()
    {
        public TestOptionsManager() : this(new TOptions()) { }
        public TestOptionsManager(TOptions value) { Value = value; }
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
                new DataAnnotationsMetadataProvider(new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(),stringLocalizerFactory),
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
                new DataAnnotationsMetadataProvider(new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(),stringLocalizerFactory: null),
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
                      new DataAnnotationsMetadataProvider(new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(),stringLocalizerFactory: null),
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
        public FakeUrlHelper(ActionContext actionContext){ ActionContext = actionContext; }

        public string Action(UrlActionContext actionContext){return actionContext.Action; }

        public string Content(string contentPath){return $"<html><head></head><body>Content from {contentPath}</body></html>";}

        public bool IsLocalUrl(string url) => true;

        public string RouteUrl(UrlRouteContext routeContext){ return $"{routeContext.Protocol}://{routeContext.Host}/{routeContext.RouteName}";}

        public string Link(string routeName, object values){return routeName + "?" + String.Join("&", new RouteValueDictionary(values).Select(p=> p.Key+"="+p.Value));}

        public ActionContext ActionContext { get; }
    }
}
#endif