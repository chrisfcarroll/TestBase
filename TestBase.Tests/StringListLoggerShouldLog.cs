using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace TestBase.Tests
{
    [TestFixture]
    public class StringListLoggerShould
    {
        [Test]
        public void Log()
        {
            var uut= new StringListLogger();
            //
            uut.LogInformation("Information!");
            uut.LogError("Error!");
            //
            uut.LoggedLines.ShouldBeOfLength(2);
            uut.LoggedLines[0].ShouldContain("Information!");
            uut.LoggedLines[1].ShouldContain("Error!");
        }
    }
}
