namespace Serilog.Assert;

public static partial class LogAssert
{
    /// <param name="action">The method or member call being logged.</param>
    /// <param name="label"></param>
    /// <returns>
    /// <paramref name="label"/> if it is not null, empty, or the same as
    /// <paramref name="action"/>, otherwise empty string.
    /// </returns>
    internal static string StateLabelIfHelpful(string action, string? label)
        => (label is null or ""
            || label == action
            || label.EndsWith(".ToLoggableState()")
            )
                ? ""
                : label + "=";

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns>
    /// Messsage and args concatenated with comma, not actually formatted.
    /// </returns>
    internal static string PoorFormat(string message, object?[] args)
    {
        return args == null || args.Length==0
            ? message ?? ""
            : $"{message} args={string.Join(separator: ",", values: args.Select(a => a?.ToString()))}";
    }

    internal const string MessageThrew = "EnsureElseLogAndThrow threw during evaluation. {args}";
    internal const string MessageWasFalse = "EnsureElseLogAndThrow was false. {args}";
}
