using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace TestBase.RecordingStopwatch.Specs;

public class RecordsEventsInTimeSequence
{
    readonly ITestOutputHelper _output;

    [Fact]
    public void GivenTwoEventsWithASecondBetweenThem()
    {
        var uut = new RecordingStopwatch().ClearAndStart();
        uut.Add("Event1");
        Thread.Sleep(1000);
        Thread.Sleep(1);
        uut.Add("Event2");

        //
        var actual = uut.Timings;
        
        _output.WriteLine(uut.ToString());
        //
        actual.Count.ShouldBe(2);
        actual.Select(t => t.Event).ShouldEqualByValue(new[] { "Event1", "Event2" });
        actual[0].Elapsed.Ticks.ShouldBeBetween(0, actual[1].Elapsed.Ticks);
        (actual[1].Elapsed - actual[0].Elapsed).ShouldBeGreaterThan(TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void AfterClearAndStart()
    {
        var uut = new RecordingStopwatch().ClearAndStart();
        uut.Timings.ShouldBeEmpty();
        _output.WriteLine(uut.ToString());
    }
    
    public RecordsEventsInTimeSequence(ITestOutputHelper output) => _output = output;
}