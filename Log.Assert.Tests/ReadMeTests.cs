using System.Globalization;
using Microsoft.Extensions.Logging;
using TestBase;

namespace Log.Assert.Tests;

[TestFixture]
public class ReadMeTests
{
    [Test]
    public void ExampleLoggingIsCorrectGivenContractsAllPass()
    {
        new ReadMeExample(log).GoBang("12🍾4💥", 1);

        log.Entries.ForEach(TestContext.WriteLine);

        log.Entries.ShouldHaveCount(2);

        log.Entries[0].Level.ShouldBe(LogLevel.Information);
        log.Entries[0].Message
            .ShouldContain(nameof(ReadMeExample))
            .ShouldContain("GoBang");
    }

    [Test]
    public void ExampleLoggingIsCorrectGivenPreconditionsFail()
    {
        try { new ReadMeExample(log).GoBang(null!,1); }
        catch (Exception)
        {
            // The method threw. The logging will tell us why:
        }
        log.Entries.ForEach(TestContext.WriteLine);

        log.Entries.ShouldHaveCount(4);

        log.Entries[1].Message
            .ShouldContain(nameof(ReadMeExample))
            .ShouldContain("GoBang")
            .ShouldContain("Precondition Not Null Failed")
            .ShouldContain("target");
        log.Entries[1].Level.ShouldBe(LogLevel.Error);

        log.Entries[2].Message
            .ShouldContain(nameof(ReadMeExample))
            .ShouldContain("GoBang")
            .ShouldContain("Assertion Failed")
            .ShouldContain("graphemes.LengthInTextElements > 0");
        log.Entries[3].Level.ShouldBe(LogLevel.Error);

        log.Entries[3].Message
            .ShouldContain(nameof(ReadMeExample))
            .ShouldContain("GoBang")
            .ShouldContain("Precondition Failed")
            .ShouldContain(
                "0 <= guess && guess <= graphemes.LengthInTextElements"
                );
        log.Entries[3].Level.ShouldBe(LogLevel.Error);
    }

    TestLogger<ReadMeExample> log = null!;

    [SetUp]
    public void SetUp() => log = new ();
}

public class ReadMeExample(ILogger<ReadMeExample> log)
{
    public string GoBang(string target, int guess)
    {
        log.Member( (target,guess) );
        log.PreconditionNotNull(target);

        var graphemes = new StringInfo(target??"");
        log.Assert(graphemes.LengthInTextElements > 0);
        log.Precondition(0 <= guess && guess <= graphemes.LengthInTextElements);

        log.ExceptionAndThrowIf(
            graphemes.SubstringByTextElements(guess,1) is "💥",
            new ApplicationException("bang!"));

        var remainder = Remove(graphemes,guess);
        log.DebugIf(remainder.Length == 0,"TBC:is this permitted?");
        log.If(graphemes.LengthInTextElements > 0,
                  (target,guess,graphemes.LengthInTextElements),
                  label:"Remaining after Removal");

        log.PreconditionNotNull(target);
        log.Postcondition(remainder.Length < target.Length);
        return remainder;
    }

    static string Remove(StringInfo graphemes,int guess)
    {
        return graphemes.SubstringByTextElements(0, guess)
               +
               graphemes.SubstringByTextElements(guess + 1, graphemes.LengthInTextElements - guess - 1);
    }
}