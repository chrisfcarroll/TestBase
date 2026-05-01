using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

/// <summary>
/// A simple ILogger implementation that captures log entries for test assertions.
/// </summary>
public class TestLogger : ILogger
{
    public List<LogEntry> Entries { get; } = new();

    public LogEntry Last => Entries[^1];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                            Func<TState, Exception?, string> formatter)
    {
        Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
    }

    public record LogEntry(LogLevel Level, string Message, Exception? Exception);
}
