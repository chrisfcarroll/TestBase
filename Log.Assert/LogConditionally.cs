using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Log.Assert;

/// <summary>
/// ILogger extension methods for conditional logging.
/// </summary>
public static class LogConditionally
{
    /// <summary>
    /// If <paramref name="condition"/> is true, log at <param name="logLevel">.
    /// Defaults to <see cref="LogLevel.Information"/>
    /// </param>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="helpfulInformation">
    ///     anything you think would be helpful to see when scanning the logs. Will be logged
    ///     under the property name "State".
    /// </param>
    /// <param name="label">
    ///     Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///     under the property name "Label".
    /// </param>
    /// <param name="logLevel">Defaults to <see cref="LogLevel.Information"/></param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///     the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///     Compiler generated: the expression being tested.
    /// </param>
    public static bool If(this ILogger log,
                          bool condition,
                          object? helpfulInformation = null,
                          [CallerArgumentExpression("helpfulInformation")]
                          string? label = "",
                          LogLevel logLevel = LogLevel.Information,
                          [CallerMemberName] string action = "",
                          [CallerArgumentExpression("condition")]
                          string? conditionExpression = "")
    {
        if (!condition) return false;
        if (!log.IsEnabled(logLevel)) return true;
        log.Log(logLevel,
                LogLine.Conditional,
                action,
                conditionExpression,
                LogAssert.StateLabelIfHelpful(action,label),
                helpfulInformation.ForLogging() ?? string.Empty);
        return true;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Information"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="helpfulInformation">
    ///    anything you think would be helpful to see when scanning the logs. Will be logged
    ///    under the property name "State".
    /// </param>
    /// <param name="label">
    ///    Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///    under the property name "Label".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///    the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///    Compiler generated: the expression being tested.
    /// </param>
    public static bool InformationIf(this ILogger log,
                                     bool condition,
                                     object? helpfulInformation = null,
                                     [CallerArgumentExpression("helpfulInformation")]
                                     string? label = "",
                                     [CallerMemberName] string action = "",
                                     [CallerArgumentExpression("condition")]
                                     string? conditionExpression = "")
        => If(log,
              condition,
              helpfulInformation,
              label,
              LogLevel.Information,
              action,
              conditionExpression);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Warning"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="helpfulInformation">
    ///    anything you think would be helpful to see when scanning the logs. Will be logged
    ///    under the property name "State".
    /// </param>
    /// <param name="label">
    ///    Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///    under the property name "Label".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///    the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///    Compiler generated: the expression being tested.
    /// </param>
    public static bool WarnIf(this ILogger log,
                              bool condition,
                              object? helpfulInformation = null,
                              [CallerArgumentExpression("helpfulInformation")]
                              string? label = "",
                              [CallerMemberName] string action = "",
                              [CallerArgumentExpression("condition")]
                              string? conditionExpression = "")
        => If(log,
              condition,
              helpfulInformation,
              label,
              LogLevel.Warning,
              action,
              conditionExpression);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Error"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="helpfulInformation">
    ///    anything you think would be helpful to see when scanning the logs. Will be logged
    ///    under the property name "State".
    /// </param>
    /// <param name="label">
    ///    Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///    under the property name "Label".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///    the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///    Compiler generated: the expression being tested.
    /// </param>
    public static bool ErrorIf(this ILogger log,
                               bool condition,
                               object? helpfulInformation = null,
                               [CallerArgumentExpression("helpfulInformation")]
                               string? label = "",
                               [CallerMemberName] string action = "",
                               [CallerArgumentExpression("condition")]
                               string? conditionExpression = "")
        => If(log,
              condition,
              helpfulInformation,
              label,
              LogLevel.Error,
              action,
              conditionExpression);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Debug"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="helpfulInformation">
    ///    anything you think would be helpful to see when scanning the logs. Will be logged
    ///    under the property name "State".
    /// </param>
    /// <param name="label">
    ///    Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///    under the property name "Label".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///    the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///    Compiler generated: the expression being tested.
    /// </param>
    public static bool DebugIf(this ILogger log,
                               bool condition,
                               object? helpfulInformation = null,
                               [CallerArgumentExpression("helpfulInformation")]
                               string? label = "",
                               [CallerMemberName] string action = "",
                               [CallerArgumentExpression("condition")]
                               string? conditionExpression = "",
                               [CallerLineNumber] int line = 0)
    {
        if (!condition) return false;
        if (!log.IsEnabled(LogLevel.Debug)) return true;
        log.Log(LogLevel.Debug,
                LogLine.ConditionalWithLine,
                action,
                line,
                conditionExpression,
                LogAssert.StateLabelIfHelpful(action,label),
                helpfulInformation.ForLogging() ?? string.Empty);
        return true;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Trace"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="helpfulInformation">
    ///    anything you think would be helpful to see when scanning the logs. Will be logged
    ///    under the property name "State".
    /// </param>
    /// <param name="label">
    ///    Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///    under the property name "Label".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///    the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///    Compiler generated: the expression being tested.
    /// </param>
    public static bool TraceIf(this ILogger log,
                               bool condition,
                               object? helpfulInformation = null,
                               [CallerArgumentExpression("helpfulInformation")]
                               string? label = "",
                               [CallerMemberName] string action = "",
                               [CallerArgumentExpression("condition")]
                               string? conditionExpression = "",
                               [CallerLineNumber] int line = 0)
    {
        if (!condition) return false;
        if (!log.IsEnabled(LogLevel.Trace)) return true;
        log.Log(LogLevel.Trace,
                LogLine.ConditionalWithLine,
                action,
                line,
                conditionExpression,
                LogAssert.StateLabelIfHelpful(action,label),
                helpfulInformation.ForLogging() ?? string.Empty);
        return true;
    }


    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Critical"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="helpfulInformation">
    ///    anything you think would be helpful to see when scanning the logs. Will be logged
    ///    under the property name "State".
    /// </param>
    /// <param name="label">
    ///    Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///    under the property name "Label".
    /// </param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///    the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///    Compiler generated: the expression being tested.
    /// </param>
    public static bool CriticalIf(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? label = "",
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "")
        => If(log,
              condition,
              helpfulInformation,
              label,
              LogLevel.Critical,
              action,
              conditionExpression);
}