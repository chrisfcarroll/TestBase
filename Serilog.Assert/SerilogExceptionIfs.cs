using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;

namespace SerilogAssert;

public static class SerilogExceptionIfs
{
    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="ex"/>
    /// at <see cref="LogEventLevel.Error"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="ex">
    /// The exception to log. If null, an <see cref="ApplicationException"/> will be created
    /// with message "Exception in {action}."
    /// </param>
    /// <param name="conditionExpression">
    /// Compiler generated: the expression being tested.
    /// </param>
    /// <param name="helpfulInformation">
    /// anything you think would be helpful to see when scanning the logs. Will be logged
    /// under the property name "State".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    /// the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="helpfulLabel">
    /// Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    /// under the property name "Label".
    /// </param>
    public static bool ExceptionIf(this ILogger log,
                                   bool condition,
                                   Exception? ex,
                                   object? helpfulInformation = null,
                                   [CallerMemberName] string action = "",
                                   [CallerArgumentExpression("condition")]
                                   string? conditionExpression = "",
                                   [CallerArgumentExpression("helpfulInformation")]
                                   string? helpfulLabel = "")
    {
        if (!condition) return false;
        if (!log.IsEnabled(LogEventLevel.Error)) return true;
        ex ??= new ApplicationException(message: $"Exception in {action}");
        log.Write(LogEventLevel.Error,
                  ex,
                  "{Action}:Condition Failed:{Condition}:{Label}{State}",
                  action,
                  conditionExpression,
                  SerilogAssertions.StateLabelIfHelpful(action,helpfulLabel),
                  helpfulInformation.ToLoggableState() ?? string.Empty);
        return true;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="ex"/>
    /// at <see cref="LogEventLevel.Error"/> then throw it.
    /// Otherwise, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="ex">
    /// The exception to log. If null, an <see cref="ApplicationException"/> will be created
    /// with message "Exception in {action}."
    /// </param>
    /// <param name="conditionExpression">
    /// Compiler generated: the expression being tested.
    /// </param>
    /// <param name="helpfulInformation">
    /// anything you think would be helpful to see when scanning the logs. Will be logged
    /// under the property name "State".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    /// the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="helpfulLabel">
    /// Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    /// under the property name "Label".
    /// </param>
    public static bool ExceptionAndThrowIf(this ILogger log,
                                           bool condition,
                                           Exception? ex,
                                           object? helpfulInformation = null,
                                           [CallerMemberName] string action = "",
                                           [CallerArgumentExpression("condition")]
                                           string? conditionExpression = "",
                                           [CallerArgumentExpression("helpfulInformation")]
                                           string? helpfulLabel = "")
    {
        if (!condition) return false;
        ex ??= new ApplicationException(message: $"Exception in {action}");
        if (log.IsEnabled(LogEventLevel.Error))
            log.Write(LogEventLevel.Error,
                      ex,
                      "{Action}:Condition Failed:{Condition}:{Label}{State}",
                      action,
                      conditionExpression,
                      SerilogAssertions.StateLabelIfHelpful(action,helpfulLabel),
                      helpfulInformation.ToLoggableState() ?? string.Empty);
        throw ex;
    }
}
