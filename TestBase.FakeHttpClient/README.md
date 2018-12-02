*TestBase* gives you a flying start with 
- fluent assertions that are simple to extend
- sharp error messages
- tools to help you test with “heavyweight” dependencies on 
  - AspNetCore.Mvc, AspNet.Mvc 3-5, or WebApi Contexts
  - HttpClient
  - Ado.Net
  - Streams & Logging
- Mix & match with your favourite test runners & assertions.

# TestBase.HttpClient.Fake

```
//Arrange
var httpClient = new FakeHttpClient()
                .SetupGetUrl("https://host.*/").Returns(request=> "Got:" + request.RequestUri)
                
                .SetupGetPath("/uri[Pp]attern/").Returns("stringcontent")
                
                .SetupPost(".*").Returns(response)

                .SetupPost(".*", new byte[]{1,2,3}).Returns(otherResponse)

                .SetupPost(".*", "a=1&b=2")
                .Returns(
                            request => "You said : " + request.Content.ReadAsStringAsync().ConfigureFalseGetResult(),
                            HttpStatusCode.Accepted)

                .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/this")).Returns(response)
                
                .Setup(x=>x.Method ==HttpMethod.Put)
                .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));

// Act
var putResponse = await httpClient.PutAsync("http://localhost/thing", new StringContent("{a=1,b=2}"));
var postResponse= await httpClient.PostAsync("http://[::1]/", new StringContent("a=1&b=2"));

//Debug
httpClient.Invocations
            .ForEach(async i =>Console.WriteLine("{0} {1}",i.RequestUri, 
                                                await i.Content.ReadAsStringAsync()));

            
//Assert
putResponse.StatusCode.ShouldBe(HttpStatusCode.Accepted);
postResponse.ShouldBe(response); // ==> SetupPost(".*").Returns(response) was the first 
                                    // matched setup. Setups are tried in first-to-last order.                                            

httpClient.Verify(x=>x.Method ==HttpMethod.Put, "Expected Put, but no matching invocations.");
httpClient.Verify(
                    x=>x.Method ==HttpMethod.Post 
                    && x.Content.ReadAsStringAsync().ConfigureFalseGetResult()=="a=1&b=2",
                    "Expected Post a=1&b=2");

httpClient.VerifyAll(); // ==> "Exception : 4 unmatched expectations"

```

### TestBase

Chainable fluent assertions get you to the point concisely.

```
UnitUnderTest.Action()
  .ShouldNotBeNull()
  .ShouldEqualByValueExceptFor(new {Id=1, Descr=expected}, ignoreList )
  .Payload
  .ShouldMatchIgnoringCase("I expected this")
	.Should(someOtherPredicate);

.ShouldEqualByValue().ShouldEqualByValueExceptFor(...).ShouldEqualByValueOnMembers()
  work with all kinds of object and collections, and report what differed.
string.ShouldMatch(pattern).ShouldNotMatch().ShouldBeEmpty().ShouldNotBeEmpty()
.ShouldNotBeNullOrEmptyOrWhiteSpace().ShouldEqualIgnoringCase()
.ShouldContain().ShouldStartWith().ShouldEndWith().ShouldBeContainedIn().ShouldBeOneOf().ShouldNotBeOneOf()
numeric.ShouldBeBetween().ShouldEqualWithTolerance()....GreaterThan....LessThan...GreaterOrEqualTo ...
ienumerable.ShouldAll().ShouldContain().ShouldNotContain().ShouldBeEmpty().ShouldNotBeEmpty() ...
stream.ShouldHaveSameStreamContentAs().ShouldContain()
value.ShouldBe().ShouldNotBe().ShouldBeOfType().ShouldBeAssignableTo()...
.ShouldAll(predicate), .SingleOrAssertFail()...

```

See also
 - [TestBase](https://www.nuget.org/packages/TestBase)
 - [TestBase.AspNetCore.Mvc](https://www.nuget.org/packages/TestBase.AspNetCore.Mvc)
 - [TestBase-Mvc](https://www.nuget.org/packages/TestBase-Mvc)
 - [TestBase.AdoNet](https://www.nuget.org/packages/TestBase.AdoNet)
 - [Serilog.Sinks.ListOfString](https://www.nuget.org/packages/Serilog.Sinks.Listofstring)
 - [Extensions.Logging.ListOfString](https://www.nuget.org/packages/Extensions.Logging.ListOfString)
