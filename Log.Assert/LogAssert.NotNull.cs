using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Log.Assert;

public static partial class LogAssert
{
    [return:NotNull]
    public static string AssertNotNullOrEmpty(this ILogger log,
                                              [NotNull] string? it,
                                              object? helpfulInformation = null,
                                              [CallerMemberName] string action = "",
                                              [CallerArgumentExpression("it")]
                                              string? subject = null,
                                              [CallerArgumentExpression("helpfulInformation")]
                                              string? helpfulLabel = "")
    {
        if (string.IsNullOrEmpty(it))
        {
            log.LogError("{Action}:Assertion Not Null Or Empty Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action, helpfulLabel),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }

    [return:NotNull]
    public static string AssertNotNullOrWhitespace(this ILogger log,
                                                    [NotNull] string? it,
                                                    object? helpfulInformation = null,
                                                    [CallerMemberName] string action = "",
                                                    [CallerArgumentExpression("it")]
                                                    string? subject = null,
                                                    [CallerArgumentExpression("helpfulInformation")]
                                                    string? helpfulLabel = "")
    {
        if (string.IsNullOrWhiteSpace(it))
        {
            log.LogError("{Action}:Assertion Not Null Or Whitespace Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action, helpfulLabel),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }

    [return:NotNull]
    public static string PreconditionNotNullOrEmpty(this ILogger log,
                                                     [NotNull] string? it,
                                                     object? helpfulInformation = null,
                                                     [CallerMemberName] string action = "",
                                                     [CallerArgumentExpression("it")]
                                                     string? subject = null,
                                                     [CallerArgumentExpression("helpfulInformation")]
                                                     string? helpfulLabel = "")
    {
        if (string.IsNullOrEmpty(it))
        {
            log.LogError("{Action}:Precondition Not Null Or Empty Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action, helpfulLabel),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }

    [return:NotNull]
    public static string PreconditionNotNullOrWhitespace(this ILogger log,
                                                          [NotNull] string? it,
                                                          object? helpfulInformation = null,
                                                          [CallerMemberName] string action = "",
                                                          [CallerArgumentExpression("it")]
                                                          string? subject = null,
                                                          [CallerArgumentExpression("helpfulInformation")]
                                                          string? helpfulLabel = "")
    {
        if (string.IsNullOrWhiteSpace(it))
        {
            log.LogError("{Action}:Precondition Not Null Or Whitespace Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action, helpfulLabel),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }

    [return:NotNull]
    public static string PostconditionNotNullOrEmpty(this ILogger log,
                                                      [NotNull] string? it,
                                                      object? helpfulInformation = null,
                                                      [CallerMemberName] string action = "",
                                                      [CallerArgumentExpression("it")]
                                                      string? subject = null,
                                                      [CallerArgumentExpression("helpfulInformation")]
                                                      string? helpfulLabel = "")
    {
        if (string.IsNullOrEmpty(it))
        {
            log.LogError("{Action}:Postcondition Not Null Or Empty Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action, helpfulLabel),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }

    [return:NotNull]
    public static string PostconditionNotNullOrWhitespace(this ILogger log,
                                                           [NotNull] string? it,
                                                           object? helpfulInformation = null,
                                                           [CallerMemberName] string action = "",
                                                           [CallerArgumentExpression("it")]
                                                           string? subject = null,
                                                           [CallerArgumentExpression("helpfulInformation")]
                                                           string? helpfulLabel = "")
    {
        if (string.IsNullOrWhiteSpace(it))
        {
            log.LogError("{Action}:Postcondition Not Null Or Whitespace Failed:{Subject}:{Label}{State}",
                         action,
                         subject,
                         StateLabelIfHelpful(action, helpfulLabel),
                         helpfulInformation.ForLogging() ?? string.Empty);
        }
        #pragma warning disable CS8777
        return it!;
        #pragma warning restore CS8777
    }
}
