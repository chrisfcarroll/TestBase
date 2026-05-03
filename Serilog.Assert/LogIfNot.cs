using System.Runtime.CompilerServices;
using Serilog.Events;

namespace Serilog.Assert;

/// <summary>
/// ILogger extension methods for conditional logging when condition is false.
/// </summary>
public static class LogIfNot
{
    public static bool IfNot(this ILogger log,
                             bool condition,
                             object? helpfulInformation = null,
                             LogEventLevel logLevel = LogEventLevel.Information,
                             [CallerMemberName] string action = "",
                             [CallerArgumentExpression("condition")]
                             string? conditionExpression = "",
                             [CallerArgumentExpression("helpfulInformation")]
                             string? label = "")
    {
        if (condition) return true;
        if (!log.IsEnabled(logLevel)) return false;
        log.Write(logLevel,
                  LogLine.Conditional,
                  action,
                  conditionExpression,
                  LogAssert.StateLabelIfHelpful(action, label),
                  helpfulInformation.ForLogging() ?? string.Empty);
        return false;
    }

    public static bool InformationIfNot(this ILogger log,
                                        bool condition,
                                        object? helpfulInformation = null,
                                        [CallerMemberName] string action = "",
                                        [CallerArgumentExpression("condition")]
                                        string? conditionExpression = "",
                                        [CallerArgumentExpression("helpfulInformation")]
                                        string? label = "")
        => IfNot(log, condition, helpfulInformation, LogEventLevel.Information, action, conditionExpression, label);

    public static bool WarnIfNot(this ILogger log,
                                 bool condition,
                                 object? helpfulInformation = null,
                                 [CallerMemberName] string action = "",
                                 [CallerArgumentExpression("condition")]
                                 string? conditionExpression = "",
                                 [CallerArgumentExpression("helpfulInformation")]
                                 string? label = "")
        => IfNot(log, condition, helpfulInformation, LogEventLevel.Warning, action, conditionExpression, label);

    public static bool ErrorIfNot(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "",
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? label = "")
        => IfNot(log, condition, helpfulInformation, LogEventLevel.Error, action, conditionExpression, label);

    public static bool DebugIfNot(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "",
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? label = "",
                                  [CallerLineNumber] int line = 0)
    {
        if (condition) return true;
        if (!log.IsEnabled(LogEventLevel.Debug)) return false;
        log.Write(LogEventLevel.Debug,
                  LogLine.ConditionalWithLine,
                  action,
                  line,
                  conditionExpression,
                  LogAssert.StateLabelIfHelpful(action, label),
                  helpfulInformation.ForLogging() ?? string.Empty);
        return false;
    }

    public static bool VerboseIfNot(this ILogger log,
                                    bool condition,
                                    object? helpfulInformation = null,
                                    [CallerMemberName] string action = "",
                                    [CallerArgumentExpression("condition")]
                                    string? conditionExpression = "",
                                    [CallerArgumentExpression("helpfulInformation")]
                                    string? label = "",
                                    [CallerLineNumber] int line = 0)
    {
        if (condition) return true;
        if (!log.IsEnabled(LogEventLevel.Verbose)) return false;
        log.Write(LogEventLevel.Verbose,
                  LogLine.ConditionalWithLine,
                  action,
                  line,
                  conditionExpression,
                  LogAssert.StateLabelIfHelpful(action, label),
                  helpfulInformation.ForLogging() ?? string.Empty);
        return false;
    }

    public static bool FatalIfNot(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "",
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? label = "")
        => IfNot(log, condition, helpfulInformation, LogEventLevel.Fatal, action, conditionExpression, label);
}
