using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace LogAssert;

/// <summary>
/// ILogger extension methods for conditional logging, assertions, and action logging.
/// </summary>
public static partial class LogAssertions
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
            log.LogError(message: "Tried to log a null exception {args}", args);
            return exception;
        }

        log.LogError(exception, message: exception.GetType().ToString(), args);
        return exception;
    }

    /// <summary>
    /// Log <paramref name="exception" /> and then throw it.
    /// </summary>
    [DoesNotReturn]
    public static void LogExceptionThenThrow(this ILogger log,
                                             Exception? exception,
                                             params object[] args)
    {
        exception ??= new Exception(message: "Tried to log a null exception");
        log.LogError(exception, message: exception.Message, args);
        throw exception;
    }

    /// <summary>
    /// Log <paramref name="exception" /> at level <see cref="LogLevel.Error"/>,
    /// and then throw it.
    /// </summary>
    [DoesNotReturn]
    public static void LogExceptionThenThrow(this ILogger log,
                                             Exception? exception,
                                             string? msg = null,
                                             params object[] args)
    {
        exception ??= new Exception(message: "Tried to log a null exception");
        log.LogError(exception, message: msg ?? exception.Message, args);
        throw exception;
    }

    /// <summary>Log <paramref name="exception" /> and then <see cref="Environment.Exit" /> the current Process.</summary>
    [DoesNotReturn]
    public static void LogExceptionThenSystemExitProcessWithCode(this ILogger log,
                                                                 Exception? exception,
                                                                 int exitCode,
                                                                 params object[] args)
    {
        if (exception is null)
            log.LogError(message: "Tried to log a null exception {args}", args);
        else
            log.LogError(exception, message: exception.GetType().ToString(), args);

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
            log.LogError(message: "Tried to log an empty exception");
            return null;
        }

        ApplicationException ex = new(message);
        log.LogError(ex, message: message ?? "{args}", args);
        return ex;
    }

    internal static string PoorFormat(string message, object?[] args)
    {
        return args == null || args.Length==0
            ? message ?? ""
            : $"{message} args={string.Join(separator: ",", values: args.Select(a => a?.ToString()))}";
    }

    internal const string MessageThrew = "EnsureElseLogAndThrow threw during evaluation. {args}";
    internal const string MessageWasFalse = "EnsureElseLogAndThrow was false. {args}";
}
