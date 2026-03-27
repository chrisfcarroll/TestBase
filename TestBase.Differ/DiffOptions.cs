using System.Reflection;

namespace TestBase;

/// <summary>
/// Options controlling how Differ.Diff() compares objects.
/// </summary>
public record DiffOptions
{
    public static readonly DiffOptions Default = new();

    /// <summary>Tolerance for floating-point comparison.</summary>
    public double FloatTolerance { get; init; } = 1e-14d;

    /// <summary>Member names to exclude from comparison (supports dotted paths like "Address.ZipCode").</summary>
    public IReadOnlyList<string> ExcludeMembers { get; init; } = [];

    /// <summary>If non-empty, only these members are compared (supports dotted paths).</summary>
    public IReadOnlyList<string> IncludeOnlyMembers { get; init; } = [];

    /// <summary>If true, types must match exactly, not just structurally.</summary>
    public bool RequireSameType { get; init; }

    /// <summary>Maximum number of differences to report before stopping (0 = first 2).</summary>
    public int MaxDifferences { get; init; } = 2;

    /// <summary>Maximum recursion depth.</summary>
    public int MaxDepth { get; init; } = 10;

    /// <summary>Label for the left operand in output.</summary>
    public string LeftLabel { get; init; } = "Expected";

    /// <summary>Label for the right operand in output.</summary>
    public string RightLabel { get; init; } = "Actual";

    /// <summary>Whether to compare only public readable properties (true) or all properties and public fields (false).</summary>
    public bool PublicPropertiesOnly { get; init; }

    /// <summary>Whether to compare only writable properties.</summary>
    public bool WritablePropertiesOnly { get; init; }

    /// <summary>Whether null is seen as equal to DBNull. Default true.</summary>
    public bool NullEqualsDbNull { get; init; } = true;

    public DiffOptions WithExclusions(params string[] members)
        => this with { ExcludeMembers = members };

    public DiffOptions WithIncludeOnly(params string[] members)
        => this with { IncludeOnlyMembers = members };

    public DiffOptions WithTolerance(double tolerance)
        => this with { FloatTolerance = tolerance };

    public DiffOptions WithLabels(string left, string right)
        => this with { LeftLabel = left, RightLabel = right };
}
