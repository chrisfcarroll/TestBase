using System.Text;

namespace TestBase;

/// <summary>
/// Formats DiffResult with optional ANSI colour output for console display.
/// Colour output is controlled by the static <see cref="UseColour"/> property.
/// </summary>
public static class DiffFormatter
{
    /// <summary>
    /// Global switch to enable/disable ANSI colour output. Default: false.
    /// Set to true to get coloured diff output on supporting consoles.
    /// </summary>
    public static bool UseColour { get; set; }

    const string Red = "\x1b[31m";
    const string Green = "\x1b[32m";
    const string Yellow = "\x1b[33m";
    const string Cyan = "\x1b[36m";
    const string Reset = "\x1b[0m";
    const string Bold = "\x1b[1m";
    const string Dim = "\x1b[2m";

    /// <summary>
    /// Format a DiffResult as a string, optionally with ANSI colours.
    /// </summary>
    public static string Format(DiffResult result)
    {
        if (result.AreEqual) return UseColour ? $"{Green}Equal{Reset}" : "Equal";
        var sb = new StringBuilder();
        FormatNode(sb, result, indent: 0);
        return sb.ToString();
    }

    static void FormatNode(StringBuilder sb, DiffResult result, int indent)
    {
        if (result.AreEqual) return;
        var prefix = new string(' ', indent * 2);

        if (!string.IsNullOrEmpty(result.Message) && result.Children.Count == 0
            && result.LeftValue is null && result.RightValue is null)
        {
            sb.Append(prefix);
            if (!string.IsNullOrEmpty(result.Path))
                sb.Append(UseColour ? $"{Cyan}{result.Path}{Reset}: " : $"{result.Path}: ");
            sb.AppendLine(UseColour ? $"{Yellow}{result.Message}{Reset}" : result.Message);
            return;
        }

        if (result.LeftValue is not null || result.RightValue is not null)
        {
            sb.Append(prefix);
            if (!string.IsNullOrEmpty(result.Path))
                sb.Append(UseColour ? $"{Bold}{result.Path}{Reset}: " : $"{result.Path}: ");
            if (!string.IsNullOrEmpty(result.Message))
                sb.Append(UseColour ? $"{Dim}{result.Message}{Reset} " : $"{result.Message} ");

            var leftLabel = result.LeftLabel ?? "Expected";
            var rightLabel = result.RightLabel ?? "Actual";
            var leftVal = result.LeftValue ?? "null";
            var rightVal = result.RightValue ?? "null";

            if (UseColour)
                sb.AppendLine($"{Red}{leftLabel} = {leftVal}{Reset}, {Green}{rightLabel} = {rightVal}{Reset}");
            else
                sb.AppendLine($"{leftLabel} = {leftVal}, {rightLabel} = {rightVal}");
            return;
        }

        if (result.Children.Count > 0)
        {
            if (!string.IsNullOrEmpty(result.Path) || !string.IsNullOrEmpty(result.Message))
            {
                sb.Append(prefix);
                if (!string.IsNullOrEmpty(result.Path))
                    sb.Append(UseColour ? $"{Bold}{result.Path}{Reset}" : result.Path);
                if (!string.IsNullOrEmpty(result.Message))
                    sb.Append(UseColour ? $": {Yellow}{result.Message}{Reset}" : $": {result.Message}");
                sb.AppendLine();
            }
            foreach (var child in result.Children)
                FormatNode(sb, child, indent + (string.IsNullOrEmpty(result.Path) ? 0 : 1));
        }
    }
}
