#if NETCOREAPP2_0
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace TestBase.Tests.AspNetCoreMVC
{
    [TestFixture]
    public class WhenUsingTestServerBuilder
    {
        [Test]
        public void Should_get_services_configuredbyStartupClass()
        {
            var server= TestServerBuilder.RunningServerUsingStartup<Mvc.AspNetCore.GuineaPig.Startup>();
            server.Host.Services
                  .GetService(typeof(ILogger<WhenUsingTestServerBuilder>))
                  .ShouldBeOfType<Logger<WhenUsingTestServerBuilder>>();
        }

        [Test]
        public void Should_correctly_guess_contentRoot_from_Startup_AssemblyName()
        {
            Assert.DoesNotThrow(
                ()=>
                    TestServerBuilder
                        .RunningServerUsingStartupAndContentRoot<Mvc.AspNetCore.GuineaPig.Startup>(
                            null,
                            "Development",null));
        }
    }
}
#endif