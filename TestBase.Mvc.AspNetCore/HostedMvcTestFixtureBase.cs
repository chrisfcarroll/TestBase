using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;

namespace TestBase
{
    /// <summary>
    /// <para>A test fixture which hosts the target project (project we wish to test) in an in-memory server.
    /// The TestFixture class is responsible for configuring and creating the TestServer, setting up an HttpClient to 
    /// communicate with the TestServer.Each of the integration tests uses the Client property to connect to the 
    /// test server and make a request.</para>
    /// Usage
    /// <para>Given an AspNetCore MVC project with a <code>Startup</code> class, a <see cref="Controller"/>, and routing, a TestFixture which inherits
    /// from <see cref="HostedMvcTestFixtureBase"/> can test Controller actions like this:</para>
    /// <code>
    /// [TestCase("/dummy/action?id={id}")]
    /// public async Task Get_Should_ReturnActionResult(string url)
    /// {
    ///   var id=Guid.NewGuid();
    ///   var httpClient=GivenClientForRunningServer&lt;Startup&gt;();
    ///   GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");
    /// 
    ///   var result= await httpClient.GetAsync(url.Formatz(new {id}));
    /// 
    ///   result
    ///     .ShouldBe_200Ok()
    ///     .Content.ReadAsStringAsync().Result
    ///     .ShouldBe("Content");
    /// }
    /// 
    /// [TestCase("/dummy")]
    /// public async Task Put_Should_ReturnA(string url)
    /// {
    ///   var something= new Fixture().Create&lt;Something&gt;();
    ///   var jsonBody= new StringContent(something.ToJSon(), Encoding.UTF8, "application/json");
    ///   var httpClient=GivenClientForRunningServer&lt;Startup&gt;();
    ///   GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");
    /// 
    ///   var result = await httpClient.PutAsync(url, jsonBody);
    /// 
    ///   result.ShouldBe_202Accepted();
    /// }
    /// </code>
    /// </summary>
    /// <remarks>
    /// <strong>See https://github.com/chrisfcarroll/TestBase/blob/netstandard20/TestBase.Tests/AspNetCoreMVC/WhenTestingControllers.cs for an example.</strong>
    /// </remarks>
    public class HostedMvcTestFixtureBase : IDisposable
    {
        HttpClient httpClient;
        TestServer TestServer;
        Type TStartup;

        public void Dispose()
        {
            httpClient?.Dispose();
            TestServer?.Dispose();
        }

        /// <summary>
        /// Builds and runs your AspNetCore MVC application, given <see cref="TStartup"/> as the Startup class,
        /// and returns an <see cref="HttpClient"/> with which you can make http calls to the application.
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="baseAddress"></param>
        /// <param name="contentRoot">The directory path of the Project under test, assumed to be the ContentRoot for the server.
        /// If not specified, we attempt to guess by searching for a directory named typeof(TStartup).GetTypeInfo().Assembly under the solution root.
        /// </param>
        /// <returns>and <see cref="HttpClient"/> which can make requests to the application.</returns>
        public HttpClient GivenClientForRunningServer<TStartup>(string baseAddress = "http://localhost", string contentRoot = null)
        {
            this.TStartup = typeof(TStartup);
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            contentRoot = contentRoot ?? GetProjectPath(startupAssembly);

            this.TestServer = TestServerBuilder.RunningServerUsingStartup<TStartup>(contentRoot);

            this.httpClient = httpClient = TestServer.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }

        /// <summary>
        /// <param name="startupAssembly">The target project's assembly.</param>
        /// <returns>The full path to the target project.</returns>
        static string GetProjectPath(Assembly startupAssembly)
        {
            var projectUnderTestName = startupAssembly.GetName().Name;
            var pathToCurrentlyExecutingTest = System.AppDomain.CurrentDomain.BaseDirectory;

            var directoryToSearchForSolnFile = new DirectoryInfo(pathToCurrentlyExecutingTest);
            do
            {
                if (directoryToSearchForSolnFile.GetFileSystemInfos("*.sln").Any())
                {
                    return Path.GetFullPath(Path.Combine(directoryToSearchForSolnFile.FullName, projectUnderTestName));
                }

                directoryToSearchForSolnFile = directoryToSearchForSolnFile.Parent;
            } while (directoryToSearchForSolnFile.Parent != null);


            throw new Exception($"Solution root could not be located using application root {pathToCurrentlyExecutingTest}.");
        }

        /// <summary>
        /// For those addicted to GivenWhenThen formatting. Usage:
        /// <code>Given(out thing, new Thing())</code>
        /// </summary>
        public T Given<T>(out T given, T value)
        {
            return given = value;
        }

        /// <summary>
        /// For those addicted to GivenWhenThen formatting. Usage:
        /// <code>Given(out thing, ()=>new Thing())</code>
        /// </summary>
        public T Given<T>(out T given, Func<T> value)
        {
            return given = value();
        }

        /// <summary>
        /// For those addicted to GivenWhenThen formatting. Usage:
        /// <code>Given(out thing, input, i=>new Thing(i))</code>
        /// </summary>
        public T Given<T, Tinput>(out T given, Tinput input, Func<Tinput, T> value)
        {
            return given = value(input);
        }

        /// <summary>
        /// Adds the specified Headers to a <see cref="HttpClient"/> request
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="headerName"></param>
        /// <param name="headerValues"></param>
        /// <returns></returns>
        protected static HttpRequestHeaders GivenRequestHeaders(HttpClient httpClient, string headerName, params string[] headerValues)
        {
            Assert.Precondition(headerValues, x => x != null && x.Length > 0);

            foreach (var headerValue in headerValues)
            {
                httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
            }

            return httpClient.DefaultRequestHeaders;
        }
    }
}