using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace TestBase
{
    /// <summary>
    /// A test fixture which hosts the target project (project we wish to test) in an in-memory server.
    /// The TestFixture class is responsible for configuring and creating the TestServer, setting up an HttpClient to communicate with the TestServer.Each of the integration tests uses the Client property to connect to the test server and make a request.
    /// </summary>
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

        public HttpClient GivenClientForRunningServer<TStartup>(out HttpClient httpClient, string baseAddress="http://localhost")
        {
            this.TStartup = typeof(TStartup);
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath(startupAssembly);

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServices)
                .UseEnvironment("Development")
                .UseStartup(typeof(TStartup));

            TestServer = new TestServer(builder);

            this.httpClient= httpClient = TestServer.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = TStartup.GetTypeInfo().Assembly;

            // Inject a custom application part manager. Overrides AddMvcCore() because that uses TryAdd().
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));

            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
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
                    return Path.GetFullPath(Path.Combine(directoryToSearchForSolnFile.FullName,projectUnderTestName));
                }

                directoryToSearchForSolnFile = directoryToSearchForSolnFile.Parent;
            }
            while (directoryToSearchForSolnFile.Parent != null);



            throw new Exception($"Solution root could not be located using application root {pathToCurrentlyExecutingTest}.");
        }

        public T Given<T>(out T given, T value) { return given = value; }
        public T Given<T>(out T given, Func<T> value) { return given = value(); }
        public T Given<T, Tinput>(out T given, Tinput input, Func<Tinput, T> value) { return given = value(input); }

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
