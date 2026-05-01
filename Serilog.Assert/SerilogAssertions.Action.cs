using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;

namespace SerilogAssert;

// Unconditional Action methods — loggableState style
public static partial class SerilogAssertions
{
    /// <summary>
    /// Log the current Method call and any relevant
    /// parameter(s) <paramref name="loggableState"/> at the specified <paramref name="logLevel"/>.
    /// </summary>
    public static void Action(this ILogger log,
                              object? loggableState = null,
                              LogEventLevel logLevel = LogEventLevel.Information,
                              [CallerMemberName] string actionDescription = "",
                              [CallerArgumentExpression("loggableState")] string? stateName = "")
        => log.Write(logLevel, "{@Action}({@StateName}{@Value})",
                     actionDescription,
                     StateNameIfHelpful(actionDescription, stateName),
                     (loggableState ?? "").ToLoggableState());

    /// <summary>
    /// Log the current Method call at <see cref="LogEventLevel.Information"/>.
    /// </summary>
    public static void ActionInformation(this ILogger log,
                                         object? loggableState = null,
                                         [CallerMemberName] string actionDescription = "",
                                         [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.Action(loggableState, LogEventLevel.Information, actionDescription, stateName);

    /// <summary>
    /// Log the current Method call at <see cref="LogEventLevel.Debug"/>.
    /// </summary>
    public static void ActionDebug(this ILogger log,
                                   object? loggableState = null,
                                   [CallerMemberName] string actionDescription = "",
                                   [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.Action(loggableState, LogEventLevel.Debug, actionDescription, stateName);

    /// <summary>
    /// Log the current Method call at <see cref="LogEventLevel.Verbose"/>.
    /// </summary>
    public static void ActionVerbose(this ILogger log,
                                     object? loggableState = null,
                                     [CallerMemberName] string actionDescription = "",
                                     [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.Action(loggableState, LogEventLevel.Verbose, actionDescription, stateName);

    /// <summary>
    /// Log the current Method call at <see cref="LogEventLevel.Warning"/>.
    /// </summary>
    public static void ActionWarning(this ILogger log,
                                     object? loggableState = null,
                                     [CallerMemberName] string actionDescription = "",
                                     [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.Action(loggableState, LogEventLevel.Warning, actionDescription, stateName);

    /// <summary>
    /// Log the current Method call at <see cref="LogEventLevel.Error"/>.
    /// </summary>
    public static void ActionError(this ILogger log,
                                   object? loggableState = null,
                                   [CallerMemberName] string actionDescription = "",
                                   [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.Error("{Action}({StateName}{Value})",
                     actionDescription,
                     StateNameIfHelpful(actionDescription, stateName),
                     loggableState);

    /// <summary>
    /// Log <paramref name="ex"/> at <see cref="LogEventLevel.Error"/>.
    /// </summary>
    public static void ActionException(this ILogger log,
                                       Exception ex,
                                       object? loggableState = null,
                                       [CallerMemberName] string actionDescription = "",
                                       [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.Error(ex, "{Action}({StateName}{Value})",
                     actionDescription,
                     StateNameIfHelpful(actionDescription, stateName),
                     loggableState);

    /// <summary>
    /// Log <paramref name="ex"/> at <see cref="LogEventLevel.Error"/> then throw.
    /// </summary>
    [DoesNotReturn]
    public static void ActionExceptionThenThrow(this ILogger log,
                                                Exception ex,
                                                object? loggableState = null,
                                                [CallerMemberName] string actionDescription = "",
                                                [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.ExceptionThenThrow(ex, "{Action}({StateName}{Value})",
                     actionDescription,
                     StateNameIfHelpful(actionDescription, stateName),
                     loggableState ?? Array.Empty<object>());
}
