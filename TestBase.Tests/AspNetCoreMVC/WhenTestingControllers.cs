using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace TestBase.Tests.AspNetCoreMVC
{
    public class Startup
    {
        public Startup(IHostingEnvironment env){Configuration = new ConfigurationBuilder().Build();}
        public IConfigurationRoot Configuration { get; }
        public void ConfigureServices(IServiceCollection services){ services.AddMvc(); }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory){app.UseMvc();}
    }
    public class Something
    {
        public string A { get; set; }
    }
    public class DummyController : Controller
    {
        public class Model { public string Id { get; set; }}
        [HttpGet]public IActionResult Action(string id){ return Content("Content"); }
        [HttpPut]public object Put([FromBody]Something something){ return Accepted(something.A);}
    }

    [TestFixture, Category("Integration")]
    public class WhenTestingControllersUsingAspNetCoreTestTestServer : HostedMvcTestFixtureBase
    {
        [TestCase("/dummy/Get/{id}")]
        public async Task Get_Should_ReturnActionResult(string url)
        {
            Given(out var id, Guid.NewGuid());
            GivenClientForRunningServer<Startup>(out var httpClient);
            GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");
            
            var result= await httpClient.GetAsync(url.Formatz(new {id}));

            result
                .ShouldBe_200Ok()
                .Content.ReadAsStringAsync().Result
                .ShouldBe("Content");
        }

        [TestCase("/dummy/Put")]
        public async Task Put_Should_ReturnA(string url)
        {
            GivenSomethingInTheBody(out var something);
            Given(out var jsonBody, new StringContent(something.ToJSon(), Encoding.UTF8, "application/json"));
            GivenClientForRunningServer<Startup>(out var httpClient);
            GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");
            //
            (await httpClient.PutAsync(url, jsonBody))
                .ShouldBe_202Accepted()
                .Content.ReadAsStringAsync().Result.ShouldBe(something.A);
        }

        void GivenSomethingInTheBody(out Something thing)
        {
            thing= new Fixture().Create<Something>();
        }

    }
}