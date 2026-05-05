using Serilog.Core;
using Serilog.Events;

namespace Serilog.Assert.Tests;

/// <summary>
/// A simple test logger that captures log events for test assertions.
/// Uses a custom sink to collect Serilog LogEvents, then exposes them
/// as simplified LogEntry records matching the original test pattern.
/// </summary>
public class TestLogger
{
    readonly TestSink _sink = new();
    public ILogger Log { get; }

    public TestLogger()
    {
        Log = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(_sink)
            .CreateLogger();
    }

    public List<LogEntry> Entries => _sink.Entries;
    public LogEntry Last => Entries[^1];

    public record LogEntry(LogEventLevel Level, string Message, Exception? Exception);

    class TestSink : ILogEventSink
    {
        public List<LogEntry> Entries { get; } = new();

        public void Emit(LogEvent logEvent)
        {
            Entries.Add(new LogEntry(
                logEvent.Level,
                logEvent.RenderMessage(),
                logEvent.Exception));
        }
    }
}
