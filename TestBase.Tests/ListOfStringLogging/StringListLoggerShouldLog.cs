using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.Logging.ListOfString;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace TestBase.Tests.ListOfStringLogging;

[TestFixture]
public class StringListLoggerShould
{
    class AnotherScope
    {
    }

    [Test]
    public void BeBuildableByLoggerFactory()
    {
            var stringListLoggerProvider = new StringListLoggerProvider();
            var factory                  = new LoggerFactory();
            factory.AddProvider(stringListLoggerProvider);
            var logger = factory.CreateLogger(GetType());

            object destructured = new {A = 1, B = "Two"};
            logger.LogInformation("This has serilog formatted fields {@Destructured}", destructured);

            var uut = StringListLogger.Instance;
            uut.LoggedLines.ForEach(Console.WriteLine);
            uut.LoggedLines.ShouldBeOfLength(1).ToList()[0].ShouldMatch(@"B\s*=\s*""?Two""?");
        }

    [Test]
    public void BeBuildableByLoggerFactoryGivenBackingList()
    {
            var loggedLines = new List<string>();
            var wrapped     = new LoggerFactory().AddStringListLogger(loggedLines).CreateLogger(nameof(Destructure));

            wrapped.LogInformation("This has destructured output {@Destructured}", new {A = 1, B = "Two"});

            loggedLines.ForEach(Console.WriteLine);
            loggedLines.ShouldBeOfLength(1).Single().ReplaceWith("", " ", "\"").ShouldMatch(@"\{A=1,B=Two\}");

            StringListLogger.Instance.LoggedLines.ShouldBe(loggedLines);
        }

    [Test]
    public void BeBuildableByLoggerFactoryGivenInstance()
    {
            var uut     = new StringListLogger();
            var wrapped = new LoggerFactory().AddStringListLogger(uut).CreateLogger<StringListLoggerShould>();

            wrapped.LogInformation("This has destructured output {@Destructured}", new {A = 1, B = "Two"});

            uut.LoggedLines.ForEach(Console.WriteLine);

            uut.LoggedLines.ShouldBeOfLength(1).Single().ReplaceWith("", " ", "\"").ShouldMatch(@"\{A=1,B=Two\}");
        }

    [Test]
    public void Destructure()
    {
            var uut     = new StringListLogger();
            var wrapped = new LoggerFactory().AddStringListLogger(uut).CreateLogger(nameof(Destructure));

            wrapped.LogInformation("This has destructured output {@Destructured}", new {A = 1, B = "Two"});

            uut.LoggedLines.ForEach(Console.WriteLine);

            uut.LoggedLines.ShouldBeOfLength(1).Single().ReplaceWith("", " ", "\"").ShouldMatch(@"\{A=1,B=Two\}");
        }

    [Test]
    public void Log()
    {
            var uut = new StringListLogger();
            //
            uut.LogInformation("Information!");
            uut.LogError("Error!");
            //
            uut.LoggedLines.ShouldBeOfLength(2);
            uut.LoggedLines[0].ShouldContain("Information!");
            uut.LoggedLines[1].ShouldContain("Error!");
        }

    [Test]
    public void LogWithScopes()
    {
            var uut = new StringListLogger();
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
            uut.LoggedLines.Where(l => l.Contains(nameof(AnotherScope)))
               .ShouldBeOfLength(2, "Expected 2 loglines for inner scope");
            uut.LoggedLines.Where(l => l.Contains(GetType().Name))
               .ShouldBeOfLength(4, "Expected 4 loglines for outer scope");
        }

    [Test]
    public void MsLoggerShouldNotThrowWhenDestructuringNullParameters()
    {
            var logger = new LoggerFactory().AddStringListLogger().CreateLogger("Test");

            logger.LogError("This is formatted for destructuring {@Destructure}", new {A = 1, B = 2});
            Console.WriteLine("Logs with format \"{ A = 1, B = 2 }\"");

            object x = null;
            logger.LogError("This is formatted for destructuring {@Destructure}", x);
        }

    [Test]
    public void NotThrowOnDestructuring()
    {
            var uut = new StringListLogger();

            object destructured = new {A = 1, B = "Two"};
            uut.LogInformation("This has serilog formatted fields {@Destructured}", destructured);

            uut.LoggedLines.ForEach(Console.WriteLine);
            uut.LoggedLines.ShouldBeOfLength(1).ToList()[0].ShouldMatch(@"B\s*=\s*""?Two""?");
        }

    [Test]
    public void NotThrowOnNullParameters()
    {
            var loggedLines = new List<string>();
            var wrapped     = new LoggerFactory().AddStringListLogger(loggedLines).CreateLogger("TestBase");

            wrapped.LogInformation("{@Destructured}", new {A = 1, B = "Two", C = null as string});
            wrapped.LogInformation("{@A}",            null, null);
            wrapped.LogInformation("{@A}",            null as int[]);

            loggedLines.ForEach(Console.WriteLine);

            loggedLines.ShouldBeOfLength(3).ShouldAll(x => x.Matches("null"));
        }

    [Test]
    public void ShouldBeRetrievableByName()
    {
            var factory = new LoggerFactory().AddStringListLogger();
            var logger  = factory.CreateLogger("[Logger1]");

            logger.LogInformation("In Logger 1",                 new {A = 1});
            logger.LogInformation("In Logger 1 {@Destructured}", new {A = 1});

            logger = factory.CreateLogger("[Logger2]");
            logger.LogInformation("In Logger 2",                 new {A = 1});
            logger.LogInformation("In Logger 2 {@Destructured}", new {A = 1});


            StringListLogger.Instance.LoggedLines.ForEach(Console.WriteLine);
            StringListLogger.Instance.LoggedLines.ShouldBeOfLength(4).ShouldAll(s => s.Matches("In Logger"));
            StringListLogger.Instance.LoggedLines.Where(s => s.Contains("[Logger2]"))
                            .ShouldBeOfLength(2)
                            .ShouldAll(s => s.Matches("In Logger 2"));
        }
}