using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;

namespace SerilogAssert;

// If methods — loggableState style (preferred)
public static partial class SerilogAssertions
{
    /// <summary>
    /// If <paramref name="condition"/> is true, log the current Method call and
    /// <paramref name="loggableState"/> at the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool If(this ILogger log,
                          bool condition,
                          object? loggableState = null,
                          LogEventLevel logLevel = LogEventLevel.Information,
                          [CallerMemberName] string actionDescription = "",
                          [CallerArgumentExpression("loggableState")] string? stateName = "")
    {
        if (condition)
            log.Write(logLevel,
                      "{@Action}({@StateName}{@Value})",
                      actionDescription,
                      StateNameIfHelpful(actionDescription, stateName),
                      (loggableState ?? "").ToLoggableState());
        return condition;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Information"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool InformationIf(this ILogger log,
                                     bool condition,
                                     object? loggableState = null,
                                     [CallerMemberName] string actionDescription = "",
                                     [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.If(condition, loggableState, LogEventLevel.Information, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Warning"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool WarnIf(this ILogger log,
                              bool condition,
                              object? loggableState = null,
                              [CallerMemberName] string actionDescription = "",
                              [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.If(condition, loggableState, LogEventLevel.Warning, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Error"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool ErrorIf(this ILogger log,
                               bool condition,
                               object? loggableState = null,
                               [CallerMemberName] string actionDescription = "",
                               [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.If(condition, loggableState, LogEventLevel.Error, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Debug"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool DebugIf(this ILogger log,
                               bool condition,
                               object? loggableState = null,
                               [CallerMemberName] string actionDescription = "",
                               [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.If(condition, loggableState, LogEventLevel.Debug, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Verbose"/>.
    /// </summary>
    /// <returns><paramref name="condition"/></returns>
    public static bool VerboseIf(this ILogger log,
                                 bool condition,
                                 object? loggableState = null,
                                 [CallerMemberName] string actionDescription = "",
                                 [CallerArgumentExpression("loggableState")] string? stateName = null)
        => log.If(condition, loggableState, LogEventLevel.Verbose, actionDescription, stateName);

    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="ex"/> at <see cref="LogEventLevel.Error"/>.
    /// </summary>
    public static bool ExceptionIf(this ILogger log,
                                   bool condition,
                                   Exception ex,
                                   object? loggableState = null,
                                   [CallerMemberName] string actionDescription = "",
                                   [CallerArgumentExpression("loggableState")] string? stateName = null)
    {
        if (condition)
            log.Write(LogEventLevel.Error,
                      ex,
                      "{@Action}({@StateName}{@Value})",
                      actionDescription,
                      StateNameIfHelpful(actionDescription, stateName),
                      (loggableState ?? "").ToLoggableState());
        return condition;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="ex"/> at <see cref="LogEventLevel.Error"/> then throw.
    /// </summary>
    public static bool ExceptionThenThrowIf(this ILogger log,
                                            bool condition,
                                            Exception ex,
                                            object? loggableState = null,
                                            [CallerMemberName] string actionDescription = "",
                                            [CallerArgumentExpression("loggableState")] string? stateName = null)
    {
        if (condition)
            log.ExceptionThenThrow(ex,
                                   "{Action}({StateName}{Value})",
                                   actionDescription,
                                   StateNameIfHelpful(actionDescription, stateName),
                                   loggableState ?? Array.Empty<object>());
        return condition;
    }
}
