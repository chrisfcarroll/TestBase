using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Serilog.Events;

namespace Serilog.Assert;

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
                                              string? label = "")
    {
        if (string.IsNullOrEmpty(it))
        {
            log.Error(LogLine.AssertionNotNullOrEmptyFailed,
                      action,
                      subject,
                      StateLabelIfHelpful(action, label, helpfulInformation),
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
                                                    string? label = "")
    {
        if (string.IsNullOrWhiteSpace(it))
        {
            log.Error(LogLine.AssertionNotNullOrWhitespaceFailed,
                      action,
                      subject,
                      StateLabelIfHelpful(action, label, helpfulInformation),
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
                                                     string? label = "")
    {
        if (string.IsNullOrEmpty(it))
        {
            log.Error(LogLine.PreconditionNotNullOrEmptyFailed,
                      action,
                      subject,
                      StateLabelIfHelpful(action, label, helpfulInformation),
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
                                                          string? label = "")
    {
        if (string.IsNullOrWhiteSpace(it))
        {
            log.Error(LogLine.PreconditionNotNullOrWhitespaceFailed,
                      action,
                      subject,
                      StateLabelIfHelpful(action, label, helpfulInformation),
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
                                                      string? label = "")
    {
        if (string.IsNullOrEmpty(it))
        {
            log.Error(LogLine.PostconditionNotNullOrEmptyFailed,
                      action,
                      subject,
                      StateLabelIfHelpful(action, label, helpfulInformation),
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
                                                           string? label = "")
    {
        if (string.IsNullOrWhiteSpace(it))
        {
            log.Error(LogLine.PostconditionNotNullOrWhitespaceFailed,
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
