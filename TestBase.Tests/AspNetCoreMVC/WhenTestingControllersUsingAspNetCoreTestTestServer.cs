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
    class Startup
    {
        public Startup(IHostingEnvironment env){Configuration = new ConfigurationBuilder().Build();}
        IConfigurationRoot Configuration { get; }
        public void ConfigureServices(IServiceCollection services){ services.AddMvc(); }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory){app.UseMvc();}
    }

    public class Something{ public string A { get; set; } }

    [Route("dummy")]
    public class DummyController : Controller
    {
        public static Something Putted;
        public class Model { public string Id { get; set; }}

        [HttpGet("action")]public IActionResult Action(string id){ return Content("Content"); }
        [HttpPut]public object Put([FromBody]Something something){ Putted=something; return Accepted();}
    }

    [TestFixture]
    public class WhenTestingControllersUsingAspNetCoreTestTestServer : HostedMvcTestFixtureBase
    {
        [TestCase("/dummy/action?id={id}")]
        public async Task Get_Should_ReturnActionResult(string url)
        {
            var id=Guid.NewGuid();
            var httpClient=GivenClientForRunningServer<Startup>();
            GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");
            
            var result= await httpClient.GetAsync(url.Formatz(new {id}));

            result
                .ShouldBe_200Ok()
                .Content.ReadAsStringAsync().Result
                .ShouldBe("Content");
        }

        [TestCase("/dummy")]
        public async Task Put_Should_ReturnA(string url)
        {
            var something= new Fixture().Create<Something>();
            var jsonBody= new StringContent(something.ToJSon(), Encoding.UTF8, "application/json");
            var httpClient=GivenClientForRunningServer<Startup>();
            GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");

            var result = await httpClient.PutAsync(url, jsonBody);

            result.ShouldBe_202Accepted();
            DummyController.Putted.ShouldEqualByValue( something );
        }
    }
}