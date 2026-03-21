using System.Text;

namespace TestBase;

/// <summary>
/// Represents the result of comparing two values. Contains the path to the difference,
/// the left and right values, and any child differences.
/// Calling ToString() produces a clean, readable summary of what differed.
/// </summary>
public class DiffResult
{
    public static readonly DiffResult Equal = new() { AreEqual = true };

    public bool AreEqual { get; init; }
    public string Path { get; init; } = "";
    public string? LeftLabel { get; init; } = "Left";
    public string? RightLabel { get; init; } = "Right";
    public string? LeftValue { get; init; }
    public string? RightValue { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<DiffResult> Children { get; init; } = [];

    public static DiffResult Different(string path, string? leftValue, string? rightValue, string? message = null)
        => new()
        {
            AreEqual = false,
            Path = path,
            LeftValue = leftValue,
            RightValue = rightValue,
            Message = message
        };

    public static DiffResult WithChildren(string path, IReadOnlyList<DiffResult> children, string? message = null)
        => new()
        {
            AreEqual = children.Count == 0,
            Path = path,
            Children = children,
            Message = message
        };

    public DiffResult WithLabels(string left, string right)
        => new()
        {
            AreEqual = AreEqual,
            Path = Path,
            LeftLabel = left,
            RightLabel = right,
            LeftValue = LeftValue,
            RightValue = RightValue,
            Message = Message,
            Children = Children.Select(c => c.WithLabels(left, right)).ToList()
        };

    public override string ToString()
    {
        if (AreEqual) return "Equal";
        var sb = new StringBuilder();
        AppendTo(sb, indent: 0);
        return sb.ToString();
    }

    internal void AppendTo(StringBuilder sb, int indent)
    {
        var prefix = new string(' ', indent * 2);

        if (!string.IsNullOrEmpty(Message) && Children.Count == 0 && LeftValue is null && RightValue is null)
        {
            sb.Append(prefix);
            if (!string.IsNullOrEmpty(Path)) sb.Append($"{Path}: ");
            sb.AppendLine(Message);
            return;
        }

        if (LeftValue is not null || RightValue is not null)
        {
            sb.Append(prefix);
            if (!string.IsNullOrEmpty(Path)) sb.Append($"{Path}: ");
            if (!string.IsNullOrEmpty(Message)) sb.Append($"{Message} ");
            sb.AppendLine($"{LeftLabel} = {LeftValue ?? "null"}, {RightLabel} = {RightValue ?? "null"}");
            return;
        }

        if (Children.Count > 0)
        {
            if (!string.IsNullOrEmpty(Path) || !string.IsNullOrEmpty(Message))
            {
                sb.Append(prefix);
                if (!string.IsNullOrEmpty(Path)) sb.Append(Path);
                if (!string.IsNullOrEmpty(Message)) sb.Append($": {Message}");
                sb.AppendLine();
            }
            foreach (var child in Children)
            {
                child.AppendTo(sb, indent + (string.IsNullOrEmpty(Path) ? 0 : 1));
            }
        }
    }

    public static implicit operator bool(DiffResult result) => result.AreEqual;
}
