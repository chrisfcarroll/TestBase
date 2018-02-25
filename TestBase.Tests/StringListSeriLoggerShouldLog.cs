using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TestBase;

namespace TestBase.Tests
{
    [TestFixture]
    public class StringListSeriLoggerShould
    {
        [Test]
        public void LogAsSeriLogger()
        {
            var myList= new List<string>();
            var logger= myList.WrappedAsSerilogger();
            //
            logger.Information("Information!");
            logger.Error("Error!");
            //
            myList.ShouldBeOfLength(2);
            myList[0].ShouldContain("Information!");
            myList[1].ShouldContain("Error!");
        }
        
        [Test]
        public void LogAsMsiLogger()
        {
            var myList= new List<string>();
            var logger = myList.WrappedAsMsILogger();
            //
            logger.LogInformation("Information!");
            logger.LogError("Error!");
            //
            myList.ShouldBeOfLength(2);
            myList[0].ShouldContain("Information!");
            myList[1].ShouldContain("Error!");
        }
    }
}
