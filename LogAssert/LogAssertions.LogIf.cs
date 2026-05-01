using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace LogAssert;

// LogIf methods — loggableState style (preferred)
public static partial class LogAssertions
{
    /// <summary>
    /// If <paramref name="condition"/> is true, log the current Method call and
    /// <paramref name="loggableState"/> at the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool LogIf(this ILogger log,
                             bool condition,
                             object? loggableState = null,
                             LogLevel logLevel = LogLevel.Information,
                             [CallerMemberName] string actionDescription = "",
                             [CallerArgumentExpression("loggableState")] string? stateName = "")
    {
        if (condition)
            log.Log(logLevel,
                    "{@Action}({@StateName}{@Value})",
                    actionDescription,
                    StateNameIfHelpful(actionDescription, stateName),
                    (loggableState ?? "").ToLoggableState());
        return condition;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Information"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool LogInformationIf(this ILogger log,
                                        bool condition,
                                        object? loggableState = null,
                                        [CallerMemberName] string actionDescription = "",
                                        [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogIf(condition, loggableState, LogLevel.Information, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Warning"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool LogWarnIf(this ILogger log,
                                 bool condition,
                                 object? loggableState = null,
                                 [CallerMemberName] string actionDescription = "",
                                 [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogIf(condition, loggableState, LogLevel.Warning, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Error"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool LogErrorIf(this ILogger log,
                                  bool condition,
                                  object? loggableState = null,
                                  [CallerMemberName] string actionDescription = "",
                                  [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogIf(condition, loggableState, LogLevel.Error, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Debug"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool LogDebugIf(this ILogger log,
                                  bool condition,
                                  object? loggableState = null,
                                  [CallerMemberName] string actionDescription = "",
                                  [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogIf(condition, loggableState, LogLevel.Debug, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Trace"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool LogTraceIf(this ILogger log,
                                  bool condition,
                                  object? loggableState = null,
                                  [CallerMemberName] string actionDescription = "",
                                  [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogIf(condition, loggableState, LogLevel.Trace, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="ex"/> at <see cref="LogLevel.Error"/>.
    /// </summary>
    public static bool LogExceptionIf(this ILogger log,
                                      bool condition,
                                      Exception ex,
                                      object? loggableState = null,
                                      [CallerMemberName] string actionDescription = "",
                                      [CallerArgumentExpression("loggableState")] string? stateName = null)
    {
        if (condition)
            log.Log(LogLevel.Error,
                    ex,
                    "{@Action}({@StateName}{@Value})",
                    actionDescription,
                    StateNameIfHelpful(actionDescription, stateName),
                    (loggableState ?? "").ToLoggableState());
        return condition;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="ex"/> at <see cref="LogLevel.Error"/> then throw.
    /// </summary>
    public static bool LogExceptionThenThrowIf(this ILogger log,
                                               bool condition,
                                               Exception ex,
                                               object? loggableState = null,
                                               [CallerMemberName] string actionDescription = "",
                                               [CallerArgumentExpression("loggableState")] string? stateName = null)
    {
        if (condition)
            log.LogExceptionThenThrow(ex,
                                      "{Action}({StateName}{Value})",
                                      actionDescription,
                                      StateNameIfHelpful(actionDescription, stateName),
                                      loggableState ?? Array.Empty<object>());
        return condition;
    }
}
