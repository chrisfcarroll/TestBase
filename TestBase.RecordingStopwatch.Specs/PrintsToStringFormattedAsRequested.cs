using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace TestBase.RecordingStopwatch.Specs;

public class PrintsToStringFormattedAsRequested
{
    readonly ITestOutputHelper output;

    [Fact]
    public void GivenNoFormatSpecifier()
    {
        var uut = new RecordingStopwatch().ClearAndStart();
        uut.Add("Event1");
        Thread.Sleep(13);
        uut.Add("Event2");
        var time1 = uut.Timings[0].Elapsed;
        var time2 = uut.Timings[1].Elapsed;

        //
        var actual = uut.ToString();
        output.WriteLine(actual);
        
        //
        actual
            .ShouldEqual(
                string.Join("\n",
                    $"Event1 : {time1}",
                    $"Event2 : {time2}")
            );
    }
    
    [Theory]
    [InlineData("c"),InlineData("g"),InlineData("G")]
    public void GivenTimespanFormatSpecifier(string timespanFormat)
    {
        var uut = new RecordingStopwatch().ClearAndStart();
        uut.Add("Event1");
        Thread.Sleep(17);
        uut.Add("Event2");
        var time1 = uut.Timings[0].Elapsed;
        var time2 = uut.Timings[1].Elapsed;

        //
        var actual = uut.ToString(timespanFormat);
        output.WriteLine(actual);
        
        //
        actual
            .ShouldEqual(
                string.Join("\n",
                    $"Event1 : {time1.ToString(timespanFormat)}",
                    $"Event2 : {time2.ToString(timespanFormat)}")
            );
    }
    
    [Theory]
    [InlineData("F0"),InlineData("F2"),InlineData(null)]
    public void GivenFormatM_PrintsMillisecondsElapsed(string? doubleFormat)
    {
        var uut = new RecordingStopwatch().ClearAndStart();
        uut.Add("Event1");
        Thread.Sleep(17);
        uut.Add("Event2");
        var time1 = uut.Timings[0].Elapsed;
        var time2 = uut.Timings[1].Elapsed;

        //
        var actual = uut.ToString("M",doubleFormat);
        output.WriteLine(actual);
        
        //
        actual
            .ShouldEqual(
                string.Join("\n",
                    $"Event1 : {time1.TotalMilliseconds.ToString(doubleFormat)}",
                    $"Event2 : {time2.TotalMilliseconds.ToString(doubleFormat)}")
            );
    }
    
    
    [Theory]
    [InlineData("F0"),InlineData("F2"),InlineData(null)]
    public void GivenFormatS_PrintsSecondsElapsed(string? doubleFormat)
    {
        var uut = new RecordingStopwatch().ClearAndStart();
        uut.Add("Event1");
        Thread.Sleep(1001);
        uut.Add("Event2");
        var time1 = uut.Timings[0].Elapsed;
        var time2 = uut.Timings[1].Elapsed;

        //
        var actual = uut.ToString("S",doubleFormat);
        output.WriteLine(actual);
        
        //
        actual
            .ShouldEqual(
                string.Join("\n",
                    $"Event1 : {time1.TotalSeconds.ToString(doubleFormat)}",
                    $"Event2 : {time2.TotalSeconds.ToString(doubleFormat)}")
            );
    }
    
    public PrintsToStringFormattedAsRequested(ITestOutputHelper output) => this.output = output;
}