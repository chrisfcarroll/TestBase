using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestBase.RecordingStopwatch;

public class RecordingStopwatch : Stopwatch
{
    public IReadOnlyList<(string Event, TimeSpan Elapsed)> Timings => timings;
    
    public RecordingStopwatch ClearAndStart()
    {
        timings.Clear();
        Start();
        return this;
    }

    public RecordingStopwatch Add(string @event)
    {
        timings.Add((@event,Elapsed));
        return this;
    }    
    
    List<(string Event, TimeSpan Elapsed)> timings=new();
    

    /// <summary>Return the stopwatch timings as a single string, newline delimited,
    /// each row printed as "Event : Elapsed" in default TimeSpan format, hh:mm:ss.fff
    /// <seealso cref="ToString(string,string)"/>
    /// </summary>
    public override string ToString()
    {
        return string.Join("\n", timings.Select(t => $"{t.Event} : {t.Elapsed}"));
    }

    /// <summary>Return the stopwatch timings as a single string. By default the timings are newline delimited,
    /// each row printed as "Event : Elapsed" in default TimeSpan format, hh:mm:ss.fff
    /// </summary>
    /// <param name="timeSpanFormat">One of
    /// <list type="table">
    /// <item><term>M</term><description>print timespans as milliseconds formatted as <paramref name="doubleFormat"/></description></item>
    /// <item><term>S</term><description>print timespans as seconds formatted as <paramref name="doubleFormat"/></description></item>
    /// <item><term>c</term><description>print timespans as the <see cref="TimeSpan.ToString(string)"/>
    /// "c" invariant format</description>, hh:mm:ss.fffffff</item>
    /// <item><term>G</term><description>print timespans as the <see cref="TimeSpan.ToString(string)"/>
    /// "G" long format</description> dd:hh:mm:ss.ffffff</item>
    /// <item><term>g</term><description>print timespans as the <see cref="TimeSpan.ToString(string)"/>
    /// "g" short format</description>, hh:mm:ss.fff</item>
    /// </list></param>
    /// <param name="doubleFormat">If <paramref name="timeSpanFormat"/> is "M", then format the milliseconds
    /// using doubleFormat as provided by <see cref="double.ToString(string)"/>, for instance 'F2' for
    /// 2 fixed decimal places.
    /// </param>
    /// <returns></returns>
    public string ToString(string timeSpanFormat, string doubleFormat="F0")
    {
        return (timeSpanFormat) switch
        {
            "M" => string.Join("\n",
                timings.Select(t => $"{t.Event} : {t.Elapsed.TotalMilliseconds.ToString(doubleFormat)}")),
            "S" => string.Join("\n",
                timings.Select(t => $"{t.Event} : {t.Elapsed.TotalSeconds.ToString(doubleFormat)}")),
            "c" => string.Join("\n",
                timings.Select(t => $"{t.Event} : {t.Elapsed.ToString("c")}")),
            "G" => string.Join("\n",
                timings.Select(t => $"{t.Event} : {t.Elapsed.ToString("G")}")),
            _ => string.Join("\n", 
                timings.Select(t => $"{t.Event} : {t.Elapsed.ToString("g")}")),
        };
    }
}