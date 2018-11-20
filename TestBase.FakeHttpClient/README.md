*TestBase* gives you a flying start with 
- fluent assertions that are easy to extend
- sharp error messages
- tools to help you test with “heavyweight” dependencies on 
  - AspNetCore.Mvc, AspNet.Mvc or WebApi Contexts
  - HttpClient
  - Ado.Net
  - Streams & Logging
- Mix & match with your favourite test runners & assertions.

TestBase.HttpClient.Fake
------------------------

```
var httpClient = new FakeHttpClient()
      .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/this")).Returns(response)
      .Setup(x=>x.Method==HttpMethod.Put).Returns(new HttpResponseMessage(HttpStatusCode.Accepted));

...tests...;

httpClient.Verify(x=>x.Method==HttpMethod.Put);
httClient.VerifyAll();     
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

See also
 - [TestBase](https://www.nuget.org/packages/TestBase)
 - [TestBase.AspNetCore.Mvc](https://www.nuget.org/packages/TestBase.AspNetCore.Mvc)
 - [TestBase-Mvc](https://www.nuget.org/packages/TestBase-Mvc)
 - [TestBase.AdoNet](https://www.nuget.org/packages/TestBase.AdoNet)
 - [Serilog.Sinks.ListOfString](https://www.nuget.org/packages/Serilog.Sinks.Listofstring)
 - [Extensions.Logging.ListOfString](https://www.nuget.org/packages/Extensions.Logging.ListOfString)
