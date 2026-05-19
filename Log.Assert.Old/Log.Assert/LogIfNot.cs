using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Log.Assert;

/// <summary>
/// ILogger extension methods for conditional logging when condition is false.
/// </summary>
public static class LogIfNot
{
    public static bool IfNot(this ILogger log,
                             bool condition,
                             object? helpfulInformation = null,
                             [CallerArgumentExpression("helpfulInformation")]
                             string? label = "",
                             LogLevel logLevel = LogLevel.Information,
                             [CallerMemberName] string action = "",
                             [CallerArgumentExpression("condition")]
                             string? conditionExpression = "")
    {
        if (condition) return true;
        if (!log.IsEnabled(logLevel)) return false;
        log.Log(logLevel,
                LogLine.Conditional,
                action,
                conditionExpression,
                LogAssert.StateLabelIfHelpful(action, label, helpfulInformation),
                helpfulInformation.ForLogging() ?? string.Empty);
        return false;
    }

    public static bool InformationIfNot(this ILogger log,
                                        bool condition,
                                        object? helpfulInformation = null,
                                        [CallerArgumentExpression("helpfulInformation")]
                                        string? label = "",
                                        [CallerMemberName] string action = "",
                                        [CallerArgumentExpression("condition")]
                                        string? conditionExpression = "")
        => IfNot(log, condition, helpfulInformation, label, LogLevel.Information, action, conditionExpression);

    public static bool WarnIfNot(this ILogger log,
                                 bool condition,
                                 object? helpfulInformation = null,
                                 [CallerArgumentExpression("helpfulInformation")]
                                 string? label = "",
                                 [CallerMemberName] string action = "",
                                 [CallerArgumentExpression("condition")]
                                 string? conditionExpression = "")
        => IfNot(log, condition, helpfulInformation, label, LogLevel.Warning, action, conditionExpression);

    public static bool ErrorIfNot(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? label = "",
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "")
        => IfNot(log, condition, helpfulInformation, label, LogLevel.Error, action, conditionExpression);

    public static bool DebugIfNot(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? label = "",
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "",
                                  [CallerLineNumber] int line = 0)
    {
        if (condition) return true;
        if (!log.IsEnabled(LogLevel.Debug)) return false;
        log.Log(LogLevel.Debug,
                LogLine.ConditionalWithLine,
                action,
                line,
                conditionExpression,
                LogAssert.StateLabelIfHelpful(action, label, helpfulInformation),
                helpfulInformation.ForLogging() ?? string.Empty);
        return false;
    }

    public static bool TraceIfNot(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? label = "",
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "",
                                  [CallerLineNumber] int line = 0)
    {
        if (condition) return true;
        if (!log.IsEnabled(LogLevel.Trace)) return false;
        log.Log(LogLevel.Trace,
                LogLine.ConditionalWithLine,
                action,
                line,
                conditionExpression,
                LogAssert.StateLabelIfHelpful(action, label, helpfulInformation),
                helpfulInformation.ForLogging() ?? string.Empty);
        return false;
    }

    public static bool CriticalIfNot(this ILogger log,
                                     bool condition,
                                     object? helpfulInformation = null,
                                     [CallerArgumentExpression("helpfulInformation")]
                                     string? label = "",
                                     [CallerMemberName] string action = "",
                                     [CallerArgumentExpression("condition")]
                                     string? conditionExpression = "")
        => IfNot(log, condition, helpfulInformation, label, LogLevel.Critical, action, conditionExpression);
}
