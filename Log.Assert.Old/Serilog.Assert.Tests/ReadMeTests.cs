using System.Globalization;
using Serilog.Events;
using TestBase;

namespace Serilog.Assert.Tests;

[TestFixture]
public class ReadMeTests
{
    [Test]
    public void ExampleLoggingIsCorrectGivenContractsAllPass()
    {
        new ReadMeExample(log.Log).GoBang("12🍾4💥", 1);

        log.Entries.ForEach(e => TestContext.WriteLine(e));

        log.Entries.ShouldHaveCount(3);

        log.Entries[0].Level.ShouldBe(LogEventLevel.Information);
        log.Entries[0].Message
            .ShouldContain("GoBang");
    }

    [Test]
    public void ExampleLoggingIsCorrectGivenPreconditionsFail()
    {
        try { new ReadMeExample(log.Log).GoBang(null!,1); }
        catch (Exception)
        {
            // The method threw. The logging will tell us why:
        }
        log.Entries.ForEach(e => TestContext.WriteLine(e));

        log.Entries.ShouldHaveCount(4);

        log.Entries[1].Message
            .ShouldContain("GoBang")
            .ShouldContain("Precondition Not Null Failed")
            .ShouldContain("target");
        log.Entries[1].Level.ShouldBe(LogEventLevel.Error);

        log.Entries[2].Message
            .ShouldContain("GoBang")
            .ShouldContain("Assertion Failed")
            .ShouldContain("graphemes.LengthInTextElements > 0");
        log.Entries[3].Level.ShouldBe(LogEventLevel.Error);

        log.Entries[3].Message
            .ShouldContain("GoBang")
            .ShouldContain("Precondition Failed")
            .ShouldContain(
                "0 <= guess && guess <= graphemes.LengthInTextElements"
                );
        log.Entries[3].Level.ShouldBe(LogEventLevel.Error);
    }

    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new ();
}

public class ReadMeExample(ILogger log)
{
    public string GoBang(string target, int guess)
    {
        log.Member( helpfulInformation: (target,guess) );
        log.PreconditionNotNull(target);

        var graphemes = new StringInfo(target??"");
        log.Assert(graphemes.LengthInTextElements > 0);
        log.Precondition(0 <= guess && guess <= graphemes.LengthInTextElements);

        log.ExceptionAndThrowIf(
            graphemes.SubstringByTextElements(guess,1) is "💥",
            new ApplicationException("bang!"));

        var remainder = Remove(graphemes,guess);
        log.DebugIf(remainder.Length == 0,helpfulInformation: "TBC:is this permitted?");
        log.If(graphemes.LengthInTextElements > 0,
                  helpfulInformation: (target,guess,graphemes.LengthInTextElements),
                  label: "Remaining after Removal");

        log.PreconditionNotNull(target);
        log.Postcondition(remainder.Length < target.Length);
        log.Member(remainder);
        return remainder;
    }

    static string Remove(StringInfo graphemes,int guess)
    {
        return graphemes.SubstringByTextElements(0, guess)
               +
               graphemes.SubstringByTextElements(guess + 1, graphemes.LengthInTextElements - guess - 1);
    }
}
