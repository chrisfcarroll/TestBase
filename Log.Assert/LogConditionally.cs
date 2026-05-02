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
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition">the value being asserted as true</param>
    /// <param name="helpfulInformation">
    ///     anything you think would be helpful to see when scanning the logs. Will be logged
    ///     under the property name "State".
    /// </param>
    /// <param name="logLevel">Defaults to <see cref="LogLevel.Information"/></param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///     the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///     Compiler generated: the expression being tested.
    /// </param>
    /// <param name="helpfulLabel">
    ///     Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///     under the property name "Label".
    /// </param>
    public static bool If(this ILogger log,
                             bool condition,
                             object? helpfulInformation = null,
                             LogLevel logLevel = LogLevel.Information,
                             [CallerMemberName] string action = "",
                             [CallerArgumentExpression("condition")]
                             string? conditionExpression = "",
                             [CallerArgumentExpression("helpfulInformation")]
                             string? helpfulLabel = "")
    {
        if (!condition) return false;
        if (!log.IsEnabled(logLevel)) return true;
        log.Log(logLevel,
                "{Action}:{Condition}:{Label}{State}",
                action,
                conditionExpression,
                LogAssert.StateLabelIfHelpful(action,helpfulLabel),
                helpfulInformation.ForLogging() ?? string.Empty);
        return true;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Information"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="condition">the value being asserted as true</param>
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
    public static bool InformationIf(this ILogger log,
                                     bool condition,
                                     object? helpfulInformation = null,
                                     [CallerMemberName] string action = "",
                                     [CallerArgumentExpression("condition")]
                                     string? conditionExpression = "",
                                     [CallerArgumentExpression("helpfulInformation")]
                                     string? helpfulLabel = "")
        => If(log,
              condition,
              helpfulInformation,
              LogLevel.Information,
              action,
              conditionExpression,
              helpfulLabel);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Warning"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="condition">the value being asserted as true</param>
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
    public static bool WarnIf(this ILogger log,
                                 bool condition,
                                 object? helpfulInformation = null,
                                 [CallerMemberName] string action = "",
                                 [CallerArgumentExpression("condition")]
                                 string? conditionExpression = "",
                                 [CallerArgumentExpression("helpfulInformation")]
                                 string? helpfulLabel = "")
        => If(log,
              condition,
              helpfulInformation,
              LogLevel.Warning,
                 action,
                 conditionExpression,
                 helpfulLabel);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Error"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="condition">the value being asserted as true</param>
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
    public static bool ErrorIf(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "",
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? helpfulLabel = "")
        => If(log,
              condition,
              helpfulInformation,
              LogLevel.Error,
                 action,
                 conditionExpression,
                 helpfulLabel);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Debug"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="condition">the value being asserted as true</param>
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
    public static bool DebugIf(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "",
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? helpfulLabel = "")
        => If(log,
              condition,
              helpfulInformation,
              LogLevel.Debug,
                 action,
                 conditionExpression,
                 helpfulLabel);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Trace"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="condition">the value being asserted as true</param>
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
    public static bool TraceIf(this ILogger log,
                                  bool condition,
                                  object? helpfulInformation = null,
                                  [CallerMemberName] string action = "",
                                  [CallerArgumentExpression("condition")]
                                  string? conditionExpression = "",
                                  [CallerArgumentExpression("helpfulInformation")]
                                  string? helpfulLabel = "")
        => If(log,
              condition,
              helpfulInformation,
              LogLevel.Trace,
                 action,
                 conditionExpression,
                 helpfulLabel);


    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Critical"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
    /// </summary>
    /// <param name="condition">the value being asserted as true</param>
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
    public static bool CriticalIf(this ILogger log,
                                     bool condition,
                                     object? helpfulInformation = null,
                                     [CallerMemberName] string action = "",
                                     [CallerArgumentExpression("condition")]
                                     string? conditionExpression = "",
                                     [CallerArgumentExpression("helpfulInformation")]
                                     string? helpfulLabel = "")
        => If(log,
              condition,
              helpfulInformation,
              LogLevel.Critical,
                 action,
                 conditionExpression,
                 helpfulLabel);
}