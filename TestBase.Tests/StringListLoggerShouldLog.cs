using System;
using System.Linq;
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

        class AnotherScope{}

        [Test]
        public void LogWithScopes()
        {
            var uut= new StringListLogger();
            using (uut.BeginScope(this))
            {
                using (uut.BeginScope(new AnotherScope()))
                {
                    uut.LogInformation("Scope 2");
                    uut.LogError("Scope 2");
                }
                uut.LogInformation("Scope 1");
                uut.LogError("Scope 1");
            }

            uut.LoggedLines.ForEach(Console.WriteLine);

            uut.LoggedLines.ShouldBeOfLength(4, "Expected 4 loglines");
            uut.LoggedLines.Where(l => l.Contains(nameof(AnotherScope))).ShouldBeOfLength(2,"Expected 2 loglines for inner scope");
            uut.LoggedLines.Where(l => l.Contains(GetType().Name)).ShouldBeOfLength(4, "Expected 4 loglines for outer scope");
        }

        [Test]
        public void NotThrowOnSerilogDestructuring()
        {
            var uut=new StringListLogger();

            object destructured= new {A=1, B="Two"};
            uut.LogInformation("This has serilog formatted fields {@Destructured}", destructured);

            uut.LoggedLines.ForEach(Console.WriteLine);
            uut.LoggedLines.ShouldBeOfLength(1).ToList()[0].ShouldMatch(@"B\s*=\s*""?Two""?");
        }

        [Test]
        public void NotThrowOnSerilogDestructuring__EvenWhenCreatedViaMSLoggerFactory()
        {
            var stringListLoggerProvider = new StringListLoggerProvider();
            var factory=new LoggerFactory();
            factory.AddProvider(stringListLoggerProvider);
            var logger = factory.CreateLogger(GetType());

            object destructured= new {A=1, B="Two"};
            logger.LogInformation("This has serilog formatted fields {@Destructured}", destructured);

            var uut = stringListLoggerProvider.Loggers.SingleOrAssertFail("Should only have created one logger").Value;
            uut.LoggedLines.ForEach(Console.WriteLine);
            uut.LoggedLines.ShouldBeOfLength(1).ToList()[0].ShouldMatch(@"B\s*=\s*""?Two""?");
        }

        [Test]
        public void NotThrowOnNullParameters()
        {
            var uut=new StringListLogger();
            string[] args = null;

            uut.LogInformation("This has serilog formatted fields {@Destructured}", args);

            uut.LoggedLines.ShouldBeOfLength(1).Single().ShouldMatch(@"This has serilog formatted fields").ShouldMatch("null");
        }

        [Test,Ignore("Microsoft.Extensions.Logging https://github.com/aspnet/Logging/issues/767")]
        public void MsLoggerShouldNotThrowWhenDestructuringNullParameters()
        {
            var logger = new LoggerFactory().AddConsole().CreateLogger("Test");

            logger.LogError("This is formatted for destructuring {@Destructure}", new {A=1,B=2});
            Console.WriteLine("Logs with format \"{ A = 1, B = 2 }\"");

            logger.LogError("This is formatted for destructuring {@Destructure}", null);
            Console.WriteLine(@"Output:
Message: System.AggregateException : 
An error occurred while writing to logger(s). (Index (zero based) must be greater than or equal to zero and less than the size of the argument list.)
----> System.FormatException : Index (zero based) must be greater than or equal to zero and less than the size of the argument list");
        }
    }
}
