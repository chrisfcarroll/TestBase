using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Log.Assert;

/// <summary>
/// ILogger extension methods for logging Exceptions.
/// </summary>
public static class LogException
{
    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="ex"/>
    /// at <see cref="LogLevel.Error"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="ex">
    ///     The exception to log. If null, an <see cref="ApplicationException"/> will be created
    ///     with message "Exception in {action}."
    /// </param>
    /// <param name="helpfulInformation">
    ///     anything you think would be helpful to see when scanning the logs. Will be logged
    ///     under the property name "State".
    /// </param>
    /// <param name="label">
    ///     Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///     under the property name "Label".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///     the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///     Compiler generated: the expression being tested.
    /// </param>
    public static bool ExceptionIf(this ILogger log,
                                   bool condition,
                                   Exception? ex,
                                   object? helpfulInformation = null,
                                   [CallerArgumentExpression("helpfulInformation")]
                                   string? label = "",
                                   [CallerMemberName] string action = "",
                                   [CallerArgumentExpression("condition")]
                                   string? conditionExpression = "")
    {
        if (!condition) return false;
        if (!log.IsEnabled(LogLevel.Error)) return true;
        ex ??= new ApplicationException(message: $"Exception in {action}");
        log.Log(LogLevel.Error,
                ex,
                LogLine.ExceptionConditional,
                action,
                conditionExpression,
                LogAssert.StateLabelIfHelpful(action, label, helpfulInformation),
                helpfulInformation.ForLogging() ?? string.Empty);
        return true;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="ex"/>
    /// at <see cref="LogLevel.Error"/> then throw it.
    /// Otherwise, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="ex">
    ///     The exception to log. If null, an <see cref="ApplicationException"/> will be created
    ///     with message "Exception in {action}."
    /// </param>
    /// <param name="helpfulInformation">
    ///     anything you think would be helpful to see when scanning the logs. Will be logged
    ///     under the property name "State".
    /// </param>
    /// <param name="label">
    ///     Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///     under the property name "Label".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///     the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///     Compiler generated: the expression being tested.
    /// </param>
    public static bool ExceptionAndThrowIf(this ILogger log,
                                           bool condition,
                                           Exception? ex,
                                           object? helpfulInformation = null,
                                           [CallerArgumentExpression("helpfulInformation")]
                                           string? label = "",
                                           [CallerMemberName] string action = "",
                                           [CallerArgumentExpression("condition")]
                                           string? conditionExpression = "")
    {
        if (!condition) return false;
        ex ??= new ApplicationException(message: $"Exception in {action}");
        if (log.IsEnabled(LogLevel.Error))
            log.Log(LogLevel.Error,
                    ex,
                    LogLine.ExceptionConditional,
                    action,
                    conditionExpression,
                    LogAssert.StateLabelIfHelpful(action, label, helpfulInformation),
                    helpfulInformation.ForLogging() ?? string.Empty);
        throw ex;
    }
}