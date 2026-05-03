using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Log.Assert;

/// <summary>
/// ILogger extension methods to log Assertion, Precondition, and Postcondition failures.
/// </summary>
public static partial class LogAssert
{
    /// <summary>
    /// If <paramref name="assertion"/> is false, log at <see cref="LogLevel.Error"/>.
    /// Otherwise, do nothing.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="assertion">the value being asserted as true</param>
    /// <param name="assertionExpression">
    /// Compiler generated: the expression being asserted as true.
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
    public static void Assert(this ILogger log,
                              bool assertion,
                              object? helpfulInformation = null,
                              [CallerMemberName] string action = "",
                              [CallerArgumentExpression("assertion")]
                              string? assertionExpression = "",
                              [CallerArgumentExpression("helpfulInformation")]
                              string? label = "")
    {
        if (assertion) return;
        log.LogError(LogLine.AssertionFailed,
                     action,
                     assertionExpression,
                     StateLabelIfHelpful(action, label, helpfulInformation),
                     helpfulInformation.ForLogging() ?? string.Empty);
    }


    /// <summary>
    /// Log an error if <paramref name="it"/> is null.
    /// Either way, return <c>it!</c>.
    /// </summary>
    /// <remarks>
    /// For static nullability analysis, this method is equivalent to using the <c>!</c>
    /// operator.
    /// </remarks>
    /// <returns>
    /// <c>it!</c>, causing static analysis to assume that it is not null.
    /// </returns>
    [return:NotNull]
    public static T AssertNotNull<T>(this ILogger log,
                                     [NotNull] T? it,
                                     object? helpfulInformation = null,
                                     [CallerMemberName] string action = "",
                                     [CallerArgumentExpression("it")]
                                     string? subject = null,
                                     [CallerArgumentExpression("helpfulInformation")]
                                     string? label = "")
    {
        if (it is null)
        {
            log.LogError(LogLine.AssertionNotNullFailed,
                         action,
                         subject,
                         StateLabelIfHelpful(action, label, helpfulInformation),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }


    /// <summary>
    /// If <paramref name="preCondition"/> is false, log at <see cref="LogLevel.Error"/>.
    /// Otherwise, do nothing.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="preCondition">the value being asserted as true</param>
    /// <param name="assertionExpression">
    /// Compiler generated: the expression being asserted as true.
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
    public static void Precondition(this ILogger log,
                                    bool preCondition,
                                    object? helpfulInformation = null,
                                    [CallerMemberName] string action = "",
                                    [CallerArgumentExpression("preCondition")]
                                    string? assertionExpression = "",
                                    [CallerArgumentExpression("helpfulInformation")]
                                    string? label = "")
    {
        if (preCondition) return;
        log.LogError(LogLine.PreconditionFailed,
                     action,
                     assertionExpression,
                     StateLabelIfHelpful(action, label, helpfulInformation),
                     helpfulInformation.ForLogging() ?? string.Empty);
    }

    /// <summary>
    /// Log an error if <paramref name="it"/> is null.
    /// Either way, return <c>it!</c>.
    /// </summary>
    /// <remarks>
    /// For static nullability analysis, this method is equivalent to using the <c>!</c>
    /// operator.
    /// </remarks>
    /// <returns>
    /// <c>it!</c>, causing static analysis to assume that it is not null.
    /// </returns>
    [return:NotNull]
    public static T PreconditionNotNull<T>(this ILogger log,
                                            [NotNull] T? it,
                                            object? helpfulInformation = null,
                                            [CallerMemberName] string action = "",
                                            [CallerArgumentExpression("it")]
                                            string? subject = null,
                                            [CallerArgumentExpression("helpfulInformation")]
                                            string? label = "")
    {
        if (it is null)
        {
            log.LogError(LogLine.PreconditionNotNullFailed,
                         action,
                         subject,
                         StateLabelIfHelpful(action, label, helpfulInformation),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }

    /// <summary>
    /// If <paramref name="postCondition"/> is false, log at <see cref="LogLevel.Error"/>.
    /// Otherwise, do nothing.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="label"/> under the property name "Label".
    /// </summary>
    /// <param name="postCondition">the value being asserted as true</param>
    /// <param name="assertionExpression">
    /// Compiler generated: the expression being asserted as true.
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
    public static void Postcondition(this ILogger log,
                                     bool postCondition,
                                     object? helpfulInformation = null,
                                     [CallerMemberName] string action = "",
                                     [CallerArgumentExpression("postCondition")]
                                     string? assertionExpression = "",
                                     [CallerArgumentExpression("helpfulInformation")]
                                     string? label = "")
    {
        if (postCondition) return;
        log.LogError(LogLine.PostconditionFailed,
                     action,
                     assertionExpression,
                     StateLabelIfHelpful(action, label, helpfulInformation),
                     helpfulInformation.ForLogging() ?? string.Empty);
    }

    /// <summary>
    /// Log an error if <paramref name="it"/> is null.
    /// Either way, return <c>it!</c>.
    /// </summary>
    /// <remarks>
    /// For static nullability analysis, this method is equivalent to using the <c>!</c>
    /// operator.
    /// </remarks>
    /// <returns>
    /// <c>it!</c>, causing static analysis to assume that it is not null.
    /// </returns>
    [return:NotNull]
    public static T PostconditionNotNull<T>(this ILogger log,
                                        [NotNull] T? it,
                                        object? helpfulInformation = null,
                                        [CallerMemberName] string action = "",
                                        [CallerArgumentExpression("it")]
                                        string? subject = null,
                                        [CallerArgumentExpression("helpfulInformation")]
                                        string? label = "")
    {
        if (it is null)
        {
            log.LogError(LogLine.PostconditionNotNullFailed,
                         action,
                         subject,
                         StateLabelIfHelpful(action, label, helpfulInformation),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }
}