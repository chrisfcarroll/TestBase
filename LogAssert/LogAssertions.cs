using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace LogAssert;

/// <summary>
/// ILogger extension methods to log Assertion, Precondition, and Postcondition failures.
/// </summary>
public static partial class LogAssertions
{
    /// <summary>
    /// If <paramref name="assertion"/> is false, log at <see cref="LogLevel.Error"/>.
    /// Otherwise, do nothing.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
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
    /// <param name="helpfulLabel">
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
                              string? helpfulLabel = "")
    {
        if (assertion) return;
        log.LogError("{Action}:Assertion Failed:{Assertion}:{Label}{State}",
                     action,
                     assertionExpression,
                     StateLabelIfHelpful(action, helpfulLabel),
                     helpfulInformation.ToLoggableState() ?? string.Empty);
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
                                     string? helpfulLabel = "")
    {
        if (it is null)
        {
            log.LogError("{Action}:Assertion Not Null Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action,helpfulLabel),
                         helpfulInformation.ToLoggableState() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }


    /// <summary>
    /// If <paramref name="preCondition"/> is false, log at <see cref="LogLevel.Error"/>.
    /// Otherwise, do nothing.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
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
    /// <param name="helpfulLabel">
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
                                    string? helpfulLabel = "")
    {
        if (preCondition) return;
        log.LogError("{Action}:Precondition Failed:{Assertion}:{Label}{State}",
                     action,
                     assertionExpression,
                     StateLabelIfHelpful(action, helpfulLabel),
                     helpfulInformation.ToLoggableState() ?? string.Empty);
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
                                            string? helpfulLabel = "")
    {
        if (it is null)
        {
            log.LogError("{Action}:Precondition Not Null Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action,helpfulLabel),
                         helpfulInformation.ToLoggableState() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }

    /// <summary>
    /// If <paramref name="postCondition"/> is false, log at <see cref="LogLevel.Error"/>.
    /// Otherwise, do nothing.
    /// <p>Optionally also log <paramref name="helpfulInformation"/> under the property
    /// name "State", and log <paramref name="helpfulLabel"/> under the property name "Label".
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
    /// <param name="helpfulLabel">
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
                                     string? helpfulLabel = "")
    {
        if (postCondition) return;
        log.LogError("{Action}:Postcondition Failed:{Assertion}:{Label}{State}",
                     action,
                     assertionExpression,
                     StateLabelIfHelpful(action, helpfulLabel),
                     helpfulInformation.ToLoggableState() ?? string.Empty);
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
                                        string? helpfulLabel = "")
    {
        if (it is null)
        {
            log.LogError("{Action}:Postcondition Not Null Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action,helpfulLabel),
                         helpfulInformation.ToLoggableState() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }
}