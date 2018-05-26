#if NETCOREAPP2_0
using Microsoft.AspNetCore.Mvc;
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
    }
}
#endif