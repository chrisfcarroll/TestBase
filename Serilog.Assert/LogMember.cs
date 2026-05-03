using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Serilog.Events;

namespace Serilog.Assert;

/// <summary>
/// ILogger extension methods for logging the current Method call.
/// </summary>
public static class LogMember
{
    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at the specified <paramref name="logLevel"/>.
    /// Defaults to <see cref="LogEventLevel.Information"/>.
    /// </summary>
    /// <param name="helpfulInformation">
    /// anything you think would be helpful to see when scanning the logs. Will be logged
    /// under the property name "State".
    /// </param>
    /// <param name="logLevel"></param>
    /// <param name="action">Compiler populated: the name of the member (for instance,
    /// the method) being logged. Will be logged under the property name "Action"</param>
    /// <param name="label">
    /// Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    /// under the property name "Label".
    /// </param>
    public static void Member(this ILogger log,
                            object? helpfulInformation = null,
                            LogEventLevel logLevel = LogEventLevel.Information,
                            [CallerMemberName] string action = "",
                            [CallerArgumentExpression("helpfulInformation")]
                            string? label = "")
    {
        if(!log.IsEnabled(logLevel)) return;
        log.Write(logLevel,
                  LogLine.Member,
                  action,
                  LogAssert.StateLabelIfHelpful(action,label),
                  (helpfulInformation ?? "").ForLogging());
    }

    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at log level <see cref="LogEventLevel.Debug"/>.
    /// </summary>
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
    public static void MemberDebug(this ILogger log,
                                   object? helpfulInformation = null,
                                   [CallerMemberName] string action = "",
                                   [CallerArgumentExpression("helpfulInformation")]
                                   string? label = null,
                                   [CallerLineNumber] int line = 0)
    {
        if(!log.IsEnabled(LogEventLevel.Debug)) return;
        log.Write(LogEventLevel.Debug,
                  LogLine.MemberWithLine,
                  action,
                  line,
                  LogAssert.StateLabelIfHelpful(action,label),
                  (helpfulInformation ?? "").ForLogging());
    }

    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at <see cref="LogEventLevel.Verbose"/>.
    /// </summary>
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
    public static void MemberVerbose(this ILogger log,
                                     object? helpfulInformation = null,
                                     [CallerMemberName] string action = "",
                                     [CallerArgumentExpression("helpfulInformation")]
                                     string? label = null,
                                     [CallerLineNumber] int line = 0)
    {
        if(!log.IsEnabled(LogEventLevel.Verbose)) return;
        log.Write(LogEventLevel.Verbose,
                  LogLine.MemberWithLine,
                  action,
                  line,
                  LogAssert.StateLabelIfHelpful(action,label),
                  (helpfulInformation ?? "").ForLogging());
    }

    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at <see cref="LogEventLevel.Warning"/>.
    /// </summary>
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
    public static void MemberWarning(this ILogger log,
                                     object? helpfulInformation = null,
                                     [CallerMemberName] string action = "",
                                     [CallerArgumentExpression("helpfulInformation")]
                                     string? label = null)
        => Member(log,
                helpfulInformation,
                LogEventLevel.Warning,
                action,
                label);

    /// <summary>
    /// Log the name of the current Method or Member call, optionally with any additional
    /// worthwhile parameter(s) or state, at <see cref="LogEventLevel.Error"/>.
    /// </summary>
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
    public static void MemberError(this ILogger log,
                                   object? helpfulInformation = null,
                                   [CallerMemberName] string action = "",
                                   [CallerArgumentExpression("helpfulInformation")]
                                   string? label = null)
        => Member(log,
                helpfulInformation,
                LogEventLevel.Error,
                action,
                label);

    /// <summary>
    /// Log the name of the current Method or Member call, with Exception <paramref name="ex"/>,
    /// and optionally with any additional worthwhile parameter(s) or state, at
    /// <see cref="LogEventLevel.Error"/>.
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
    /// <param name="label">
    /// Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    /// under the property name "Label".
    /// </param>
    public static void MemberException(this ILogger log,
                                         Exception? ex,
                                         object? helpfulInformation = null,
                                         [CallerMemberName] string action = "",
                                         [CallerArgumentExpression("helpfulInformation")]
                                         string? label = null)
    {
        ex ??= new ApplicationException(message: $"Exception in {action}");
        if(!log.IsEnabled(LogEventLevel.Error)) return;
        log.Error(ex,
                  LogLine.MemberException,
                  action,
                  LogAssert.StateLabelIfHelpful(action,label),
                  (helpfulInformation ?? "").ForLogging());
    }

    /// <summary>
    /// Log exception <paramref name="ex"/> as occurring in the current Method or Member,
    /// and then throw it.
    /// <p>Optionally log additional worthwhile parameter(s) or state.</p>
    /// <p>Logs at log level <see cref="LogEventLevel.Error"/>.</p>
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
    /// <param name="label">
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
                                                  string? label = null)
    {
        ex ??= new ApplicationException(message: $"Exception in {action}");
        log.Error(ex,
                  LogLine.MemberException,
                  action,
                  LogAssert.StateLabelIfHelpful(action,label),
                  (helpfulInformation ?? "").ForLogging());
        throw ex;
    }


    /// <summary>
    /// Log exception <paramref name="ex"/> as a <see cref="LogEventLevel.Fatal"/> error
    /// in the current Method or Member, and then halt the currently executing process by calling
    /// <see cref="Environment.Exit" /> with <paramref name="exitCode"/>.
    /// <p>Optionally log additional worthwhile parameter(s) or state.</p>
    /// <p>Logs at log level <see cref="LogEventLevel.Fatal"/>.</p>
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
    /// <param name="label">
    /// Optional: a label to describe <paramref name="helpfulInformation"/>. Will be logged
    /// under the property name "Label".
    /// </param>
    /// <remarks>In catch clauses, prefer the <c>throw</c> statement over this method,
    /// to preserve the original stack trace.</remarks>
    /// <exception cref="Exception"><paramref name="ex"/></exception>
    /// <exception cref="Exception">If <paramref name="ex"/> is null.</exception>
    [DoesNotReturn]
    public static void MemberFatalExceptionThenExitProcessWithCode(this ILogger log,
                                                   Exception? ex,
                                                   int exitCode,
                                                   object? helpfulInformation = null,
                                                   [CallerMemberName] string action = "",
                                                   [CallerArgumentExpression("helpfulInformation")]
                                                   string? label = null)
    {
        ex ??= new ApplicationException(message: $"Terminating process with exit code {exitCode} " +
                                                 $"because Exception in {action}");
        log.Fatal(ex,
                  LogLine.MemberException,
                  action,
                  LogAssert.StateLabelIfHelpful(action,label),
                  (helpfulInformation ?? "").ForLogging());
        Environment.Exit(exitCode);
    }
}
