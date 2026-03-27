namespace TooString;

/// <summary>
/// Only used for <see cref="TooStringHow.Reflection"/>.
/// Choose between Json {"A":"B"}, DebugView { A = "B" }, or CSharp /*Type*/ new { A = "B" }
/// </summary>
public enum ReflectionStyle
{
    /// <summary>{"A":"B"} style</summary>
    Json = 0,
    /// <summary>{ A = "B" } style</summary>
    DebugView = 1,
    /// <summary>/*Type*/ new { A = "B" } style - produces valid C# syntax with type names in comments</summary>
    CSharp = 2
}