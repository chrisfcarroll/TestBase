using System.Collections.Generic;
using NUnit.Framework;
using Serilog.Sinks.ListOfString;

namespace TestBase.Tests.ListOfStringLogging
{
    [TestFixture]
    public class StringListSeriLoggerShould
    {
        [Test]
        public void LogAsSeriLogger()
        {
            var myList = new List<string>();
            var logger = myList.AsSeriLogger();
            //
            logger.Information("Information!");
            logger.Error("Error!");
            //
            myList.ShouldBeOfLength(2);
            myList[0].ShouldContain("Information!");
            myList[1].ShouldContain("Error!");
        }
    }
}
