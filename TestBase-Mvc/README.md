*TestBase* gives you a flying start with 
- fluent assertions that are easy to extend
- sharp error messages
- tools to help you test with "heavyweight" dependencies on 
    - AspNet.Mvc or AspNetCore Contexts
	- HttpClient
	- Ado.Net
	- Streams & Logging
- Mix & match with your favourite test runners and assertions.

TestBase.Mvc for Mvc4 and Mvc5
-------------------------------
Use the `Controller.WithHttpContextAndRoutes()` extension methods to fake the 
http request &amp; context, enabling you to unit test with dependecies on 
`Controller.Url()`, `Request.Cookies`, `Response.Cookies` `Request.QueryString` 
and more. By passing in your own `MvcApplication`'s actual `RegisterRoutes()` 
method , you can verify `Controller.Url` with your application's 
actual routes.

```
var controllerUnderTest = new AController().WithHttpContextAndRoutes();

controllerUnderTest
    .Action("SomeController","SomeAction",other:1)
    .ShouldBeViewWithModel<AClass>("ViewName");
        .FooterLink
        .ShouldBe("/Controller/Action?other=1");

ControllerUnderTest
  .WithHttpContextAndRoutes(
    [Optional] Action<RouteCollection> mvcApplicationRoutesRegistration, 
    [optional] string requestUrl,
    [Optional] string query = "",
    [Optional] string appVirtualPath = "/",
    [Optional] HttpApplication applicationInstance)

ApiControllerUnderTest.WithWebApiHttpContext&lt;T&gt;(
    HttpMethod httpMethod, 
    [Optional] string requestUri,
    [Optional] string routeTemplate)
```
```    
ControllerUnderTest.Action()
  .ShouldbeViewResult()
  .ShouldHaveModel<TModel>()
  .ShouldEqualByValue(expected)
ControllerUnderTest.Action()
  .ShouldBeRedirectToRouteResult()
  .ShouldHaveRouteValue("expectedKey", [Optional] "expectedValue");

ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.
```


Chainable fluent assertions get you to the point concisely:

```
UnitUnderTest.Action()
  .ShouldNotBeNull()
  .ShouldEqualByValueExceptFor(new {Id=1, Descr=expected}, ignoreList )
  .Payload
    .ShouldMatchIgnoringCase("I expected this")
	.Should(someOtherPredicate);
	.Items
      .ShouldAll(predicate)
	  .ShouldContain(item)
	  .ShouldNotContain(predicate)
	  .Where(predicate)
	  .SingleOrAssertFail()

.ShouldEqualByValue().ShouldEqualByValueExceptFor(...).ShouldEqualByValueOnMembers()
  work with all kinds of object and collections, and report what differed.
string.ShouldMatch(pattern).ShouldNotMatch().ShouldBeEmpty().ShouldNotBeEmpty()
.ShouldNotBeNullOrEmptyOrWhiteSpace().ShouldEqualIgnoringCase()
.ShouldContain().ShouldStartWith().ShouldEndWith().ShouldBeContainedIn().ShouldBeOneOf().ShouldNotBeOneOf()
numeric.ShouldBeBetween().ShouldEqualWithTolerance()....GreaterThan....LessThan...GreaterOrEqualTo ...
ienumerable.ShouldAll().ShouldContain().ShouldNotContain().ShouldBeEmpty().ShouldNotBeEmpty() ...
stream.ShouldHaveSameStreamContentAs().ShouldContain()
value.ShouldBe().ShouldNotBe().ShouldBeOfType().ShouldBeAssignableTo()...
```

See Also

TestBase.HttpClient.Fake
TestBase.AdoNet
TestBase.AspNetCore.Mvc
TestBase-Mvc for Mvc 3-5
Extensions.Logging.ListOfString
Serilog.Sinks.ListOfString
