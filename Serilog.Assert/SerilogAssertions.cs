using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;

namespace SerilogAssert;

/// <summary>
/// ILogger extension methods for conditional logging, assertions, and action logging.
/// </summary>
public static partial class SerilogAssertions
{
    internal static string StateNameIfHelpful(string actionDescription, string? stateName)
        => (stateName is null or ""
            || stateName == actionDescription
            || stateName.EndsWith(".ToLoggableState()")
           )
            ? ""
            : stateName + ":";

    /// <summary>Log <paramref name="exception" /> and return it, but don't throw it.</summary>
    [return: NotNullIfNotNull(parameterName: "exception")]
    public static Exception? Exception(this ILogger log,
                                       Exception? exception,
                                       params object[] args)
    {
        if (exception is null)
        {
            log.Error("Tried to log a null exception {args}", args);
            return exception;
        }

        log.Error(exception, exception.GetType().ToString(), args);
        return exception;
    }

    /// <summary>
    /// Log <paramref name="exception" /> and then throw it.
    /// </summary>
    [DoesNotReturn]
    public static void ExceptionThenThrow(this ILogger log,
                                          Exception exception,
                                          params object[] args)
    {
        exception ??= new Exception(message: "Tried to log a null exception");
        log.Error(exception, exception.Message, args);
        throw exception;
    }

    /// <summary>
    /// Log <paramref name="exception" /> at level <see cref="LogEventLevel.Error"/>,
    /// and then throw it.
    /// </summary>
    [DoesNotReturn]
    public static void ExceptionThenThrow(this ILogger log,
                                          Exception exception,
                                          string? msg = null,
                                          params object[] args)
    {
        exception ??= new Exception(message: "Tried to log a null exception");
        log.Error(exception, msg ?? exception.Message, args);
        throw exception;
    }

    /// <summary>Log <paramref name="exception" /> and then <see cref="Environment.Exit" /> the current Process.</summary>
    [DoesNotReturn]
    public static void ExceptionThenSystemExitProcessWithCode(this ILogger log,
                                                              Exception exception,
                                                              int exitCode,
                                                              params object[] args)
    {
        if (exception is null)
            log.Error("Tried to log a null exception {args}", args);
        else
            log.Error(exception, exception.GetType().ToString(), args);

        Environment.Exit(exitCode);
    }

    /// <summary>
    /// Create a new <see cref="ApplicationException" /> with <paramref name="message" />,
    /// log it, then return it. But don't throw it.
    /// </summary>
    public static ApplicationException? LogException(this ILogger log,
                                                     string message,
                                                     params object[] args)
    {
        if (string.IsNullOrWhiteSpace(message) && args.Length == 0)
        {
            log.Error("Tried to log an empty exception");
            return null;
        }

        ApplicationException ex = new(message);
        log.Error(ex, message ?? "{args}", args);
        return ex;
    }

    internal static string PoorFormat(string message, object?[] args)
    {
        return args == null
            ? message ?? ""
            : $"{message} args={string.Join(separator: ",", values: args.Select(a => a?.ToString()))}";
    }

    internal const string MessageThrew = "EnsureElseLogAndThrow threw during evaluation. {args}";
    internal const string MessageWasFalse = "EnsureElseLogAndThrow was false. {args}";
}
