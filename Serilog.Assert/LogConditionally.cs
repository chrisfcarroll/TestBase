using System.Runtime.CompilerServices;
using Serilog.Events;

namespace Serilog.Assert;

/// <summary>
/// ILogger extension methods for conditional logging.
/// </summary>
public static class LogConditionally
{
    /// <summary>
    /// If <paramref name="condition"/> is true, log at <param name="logLevel">.
    /// Defaults to <see cref="LogEventLevel.Information"/>
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
    /// <param name="logLevel">Defaults to <see cref="LogEventLevel.Information"/></param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    ///     the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="conditionExpression">
    ///     Compiler generated: the expression being tested.
    /// </param>
    /// <param name="label">
    ///     Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    ///     under the property name "Label".
    /// </param>
    public static bool If(this ILogger log,
                          bool condition,
                          object? helpfulInformation = null,
                          LogEventLevel logLevel = LogEventLevel.Information,
                          [CallerMemberName] string action = "",
                          [CallerArgumentExpression("condition")]
                          string? conditionExpression = "",
                          [CallerArgumentExpression("helpfulInformation")]
                          string? label = "")
    {
        if (!condition) return false;
        if (!log.IsEnabled(logLevel)) return true;
        log.Write(logLevel,
                  LogLine.Conditional,
                  action,
                  conditionExpression,
                  LogAssert.StateLabelIfHelpful(action,label),
                  helpfulInformation.ForLogging() ?? string.Empty);
        return true;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Information"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
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
    /// <param name="label">
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
                                     string? label = "")
        => If(log,
              condition,
              helpfulInformation,
              LogEventLevel.Information,
              action,
              conditionExpression,
              label);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Warning"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
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
    /// <param name="label">
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
                              string? label = "")
        => If(log,
              condition,
              helpfulInformation,
              LogEventLevel.Warning,
              action,
              conditionExpression,
              label);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Error"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
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
    /// <param name="label">
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
                               string? label = "")
        => If(log,
              condition,
              helpfulInformation,
              LogEventLevel.Error,
              action,
              conditionExpression,
              label);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Debug"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
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
    /// <param name="label">
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
                               string? label = "",
                               [CallerLineNumber] int line = 0)
    {
        if (!condition) return false;
        if (!log.IsEnabled(LogEventLevel.Debug)) return true;
        log.Write(LogEventLevel.Debug,
                  LogLine.ConditionalWithLine,
                  action,
                  line,
                  conditionExpression,
                  LogAssert.StateLabelIfHelpful(action,label),
                  helpfulInformation.ForLogging() ?? string.Empty);
        return true;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Verbose"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
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
    /// <param name="label">
    /// Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    /// under the property name "Label".
    /// </param>
    public static bool VerboseIf(this ILogger log,
                                 bool condition,
                                 object? helpfulInformation = null,
                                 [CallerMemberName] string action = "",
                                 [CallerArgumentExpression("condition")]
                                 string? conditionExpression = "",
                                 [CallerArgumentExpression("helpfulInformation")]
                                 string? label = "",
                                 [CallerLineNumber] int line = 0)
    {
        if (!condition) return false;
        if (!log.IsEnabled(LogEventLevel.Verbose)) return true;
        log.Write(LogEventLevel.Verbose,
                  LogLine.ConditionalWithLine,
                  action,
                  line,
                  conditionExpression,
                  LogAssert.StateLabelIfHelpful(action,label),
                  helpfulInformation.ForLogging() ?? string.Empty);
        return true;
    }


    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Fatal"/>.
    /// Either way, return <paramref name="condition"/>.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
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
    /// <param name="label">
    /// Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    /// under the property name "Label".
    /// </param>
    public static bool FatalIf(this ILogger log,
                               bool condition,
                               object? helpfulInformation = null,
                               [CallerMemberName] string action = "",
                               [CallerArgumentExpression("condition")]
                               string? conditionExpression = "",
                               [CallerArgumentExpression("helpfulInformation")]
                               string? label = "")
        => If(log,
              condition,
              helpfulInformation,
              LogEventLevel.Fatal,
              action,
              conditionExpression,
              label);
}
