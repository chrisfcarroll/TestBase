using Microsoft.Extensions.Logging;

namespace Log.Assert.Tests;

/// <summary>
/// A simple ILogger implementation that captures log entries for test assertions.
/// </summary>
public class TestLogger : ILogger
{
    public List<LogEntry> Entries { get; } = new();

    public LogEntry Last => Entries[^1];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                            Func<TState, Exception?, string> formatter)
    {
        Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
    }

    public record LogEntry(LogLevel Level, string Message, Exception? Exception);
}

public class TestLogger<T> : TestLogger,ILogger<T>
{
    public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                                    Func<TState, Exception?, string> formatter)
    {
        Entries.Add(new LogEntry(logLevel, typeof(T).Name + ":"+formatter(state, exception), exception));
    }

}