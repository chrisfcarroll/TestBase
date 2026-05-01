using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace LogAssert;

// Unconditional LogAction methods — loggableState style
public static partial class LogAssertions
{
    /// <summary>
    /// Log the current Method call and any relevant
    /// parameter(s) <paramref name="loggableState"/> at the specified <paramref name="logLevel"/>.
    /// </summary>
    public static void LogAction(this ILogger log,
                                 object? loggableState = null,
                                 LogLevel logLevel = LogLevel.Information,
                                 [CallerMemberName] string actionDescription = "",
                                 [CallerArgumentExpression("loggableState")] string? stateName = "")
        => log.Log(logLevel, "{@Action}({@StateName}{@Value})",
                   actionDescription,
                   StateNameIfHelpful(actionDescription, stateName),
                   (loggableState ?? "").ToLoggableState());

    /// <summary>
    /// Log the current Method call at <see cref="LogLevel.Information"/>.
    /// </summary>
    public static void LogActionInformation(this ILogger log,
                                            object? loggableState = null,
                                            [CallerMemberName] string actionDescription = "",
                                            [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogAction(loggableState, LogLevel.Information, actionDescription, stateName);

    /// <summary>
    /// Log the current Method call at <see cref="LogLevel.Debug"/>.
    /// </summary>
    public static void LogActionDebug(this ILogger log,
                                      object? loggableState = null,
                                      [CallerMemberName] string actionDescription = "",
                                      [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogAction(loggableState, LogLevel.Debug, actionDescription, stateName);

    /// <summary>
    /// Log the current Method call at <see cref="LogLevel.Trace"/>.
    /// </summary>
    public static void LogActionTrace(this ILogger log,
                                      object? loggableState = null,
                                      [CallerMemberName] string actionDescription = "",
                                      [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogAction(loggableState, LogLevel.Trace, actionDescription, stateName);

    /// <summary>
    /// Log the current Method call at <see cref="LogLevel.Warning"/>.
    /// </summary>
    public static void LogActionWarning(this ILogger log,
                                        object? loggableState = null,
                                        [CallerMemberName] string actionDescription = "",
                                        [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogAction(loggableState, LogLevel.Warning, actionDescription, stateName);

    /// <summary>
    /// Log the current Method call at <see cref="LogLevel.Error"/>.
    /// </summary>
    public static void LogActionError(this ILogger log,
                                      object? loggableState = null,
                                      [CallerMemberName] string actionDescription = "",
                                      [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogError("{Action}({StateName}{Value})",
                        actionDescription,
                        StateNameIfHelpful(actionDescription, stateName),
                        loggableState);

    /// <summary>
    /// Log <paramref name="ex"/> at <see cref="LogLevel.Error"/>.
    /// </summary>
    public static void LogActionException(this ILogger log,
                                          Exception ex,
                                          object? loggableState = null,
                                          [CallerMemberName] string actionDescription = "",
                                          [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogError(ex, "{Action}({StateName}{Value})",
                        actionDescription,
                        StateNameIfHelpful(actionDescription, stateName),
                        loggableState);

    /// <summary>
    /// Log <paramref name="ex"/> at <see cref="LogLevel.Error"/> then throw.
    /// </summary>
    [DoesNotReturn]
    public static void LogActionExceptionThenThrow(this ILogger log,
                                                   Exception ex,
                                                   object? loggableState = null,
                                                   [CallerMemberName] string actionDescription = "",
                                                   [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.LogExceptionThenThrow(ex, "{Action}({StateName}{Value})",
                        actionDescription,
                        StateNameIfHelpful(actionDescription, stateName),
                        loggableState ?? Array.Empty<object>());
}
