using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Log.Assert;

/// <summary>
/// ILogger extension methods for logging the current Method call.
/// </summary>
public static class LogMember
{
    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at the specified <paramref name="logLevel"/>.
    /// Defaults to <see cref="LogLevel.Information"/>.
    /// </summary>
    /// <param name="helpfulInformation">
    /// anything you think would be helpful to see when scanning the logs. Will be logged
    /// under the property name "State".
    /// </param>
    /// <param name="logLevel"></param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    /// the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="helpfulLabel">
    /// Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    /// under the property name "Label".
    /// </param>
    public static void Member(this ILogger log,
                               object? helpfulInformation = null,
                               LogLevel logLevel = LogLevel.Information,
                               [CallerMemberName] string action = "",
                               [CallerArgumentExpression("helpfulInformation")]
                               string? helpfulLabel = "")
    {
        if(!log.IsEnabled(logLevel)) return;
        log.Log(logLevel,
                "{Action}({Label}{@State})",
                action,
                LogAssert.StateLabelIfHelpful(action,helpfulLabel),
                (helpfulInformation ?? "").ToLoggableState());
    }

    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at log level <see cref="LogLevel.Debug"/>.
    /// </summary>
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
    public static void MemberDebug(this ILogger log,
                                      object? helpfulInformation = null,
                                      [CallerMemberName] string action = "",
                                      [CallerArgumentExpression("helpfulInformation")]
                                      string? helpfulLabel = null)
        => Member(log,
                   helpfulInformation,
                   LogLevel.Debug,
                   action,
                   helpfulLabel);

    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at <see cref="LogLevel.Trace"/>.
    /// </summary>
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
    public static void MemberTrace(this ILogger log,
                                      object? helpfulInformation = null,
                                      [CallerMemberName] string action = "",
                                      [CallerArgumentExpression("helpfulInformation")]
                                      string? helpfulLabel = null)
        => Member(log,
                   helpfulInformation,
                   LogLevel.Trace,
                   action,
                   helpfulLabel);

    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at <see cref="LogLevel.Warning"/>.
    /// </summary>
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
    public static void MemberWarning(this ILogger log,
                                        object? helpfulInformation = null,
                                        [CallerMemberName] string action = "",
                                        [CallerArgumentExpression("helpfulInformation")]
                                        string? helpfulLabel = null)
        => Member(log,
                   helpfulInformation,
                   LogLevel.Warning,
                   action,
                   helpfulLabel);

    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at <see cref="LogLevel.Error"/>.
    /// </summary>
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
    public static void MemberError(this ILogger log,
                                      object? helpfulInformation = null,
                                      [CallerMemberName] string action = "",
                                      [CallerArgumentExpression("helpfulInformation")]
                                      string? helpfulLabel = null)
        => Member(log,
                   helpfulInformation,
                   LogLevel.Error,
                   action,
                   helpfulLabel);

    /// <summary>
    /// Log the name of the current Method or Member call, with Exception <paramref name="ex"/>,
    /// and optionally with any additional worthwhile parameter(s) or state, at
    /// <see cref="LogLevel.Error"/>.
    /// </summary>
    /// <param name="ex">An exception to log as happening in method <paramref name="action"/>
    /// If null, then an <see cref="ApplicationException"/> is created with message
    /// "Exception in {action}"
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
    public static void MemberException(this ILogger log,
                                            Exception? ex,
                                            object? helpfulInformation = null,
                                            [CallerMemberName] string action = "",
                                            [CallerArgumentExpression("helpfulInformation")]
                                            string? helpfulLabel = null)
    {
        ex ??= new ApplicationException(message: $"Exception in {action}");
        if(!log.IsEnabled(LogLevel.Error)) return;
        log.LogError(ex,
                     "{Action}({Label}{State})",
                     action,
                     LogAssert.StateLabelIfHelpful(action,helpfulLabel),
                     (helpfulInformation ?? "").ToLoggableState());
    }

    /// <summary>
    /// Log exception <paramref name="ex"/> as occurring in the current Method or Member,
    /// and then throw it.
    /// <p>Optionally log additional worthwhile parameter(s) or state.</p>
    /// <p>Logs at log level <see cref="LogLevel.Error"/>.</p>
    /// </summary>
    /// <param name="ex">
    /// An exception to log and then throw.
    /// If null, then an <see cref="ApplicationException"/> is created with message
    /// "Exception in {action}"
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
    /// <remarks>In catch clauses, prefer the <c>throw</c> statement over this method,
    /// to preserve the original stack trace.</remarks>
    /// <exception cref="Exception"><paramref name="ex"/></exception>
    /// <exception cref="Exception">If <paramref name="ex"/> is null.</exception>
    [DoesNotReturn]
    public static void MemberExceptionThenThrow(this ILogger log,
                                                   Exception? ex,
                                                   object? helpfulInformation = null,
                                                   [CallerMemberName] string action = "",
                                                   [CallerArgumentExpression("helpfulInformation")]
                                                   string? helpfulLabel = null)
    {
        ex ??= new ApplicationException(message: $"Exception in {action}");
        #pragma warning disable CA1873 we're about to throw anyway.
        log.LogError(ex,
        #pragma warning restore CA1873
                     "{Action}({Label}{State})",
                     action,
                     LogAssert.StateLabelIfHelpful(action,helpfulLabel),
                     (helpfulInformation ?? "").ToLoggableState());
        throw ex;
    }


    /// <summary>
    /// Log exception <paramref name="ex"/> as a <see cref="LogLevel.Critical"/> error
    /// in the current Method or Member, and then halt the currently executing process by calling
    /// <see cref="Environment.Exit" /> with <paramref name="exitCode"/>.
    /// <p>Optionally log additional worthwhile parameter(s) or state.</p>
    /// <p>Logs at log level <see cref="LogLevel.Critical"/>.</p>
    /// </summary>
    /// <param name="ex">An exception to log and then throw.
    /// If null, then an <see cref="ApplicationException"/> is created with message
    /// "Terminating process with exit code {exitCode} because exception in {action}"
    /// </param>
    /// <param name="exitCode">The exit code to return via <see cref="Environment.Exit" />.</param>
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
    /// <remarks>In catch clauses, prefer the <c>throw</c> statement over this method,
    /// to preserve the original stack trace.</remarks>
    /// <exception cref="Exception"><paramref name="ex"/></exception>
    /// <exception cref="Exception">If <paramref name="ex"/> is null.</exception>
    [DoesNotReturn]
    public static void MemberCriticalExceptionThenExitProcessWithCode(this ILogger log,
                                                   Exception? ex,
                                                   int exitCode,
                                                   object? helpfulInformation = null,
                                                   [CallerMemberName] string action = "",
                                                   [CallerArgumentExpression("helpfulInformation")]
                                                   string? helpfulLabel = null)
    {
        ex ??= new ApplicationException(message: $"Terminating process with exit code {exitCode} " +
                                                 $"because Exception in {action}");
        #pragma warning disable CA1873 we're about to throw anyway.
        log.LogCritical(ex,
        #pragma warning restore CA1873
                     "{Action}({Label}{State})",
                     action,
                     LogAssert.StateLabelIfHelpful(action,helpfulLabel),
                     (helpfulInformation ?? "").ToLoggableState());
        Environment.Exit(exitCode);
    }
}