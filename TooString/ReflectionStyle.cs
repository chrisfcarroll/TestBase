namespace TooString;

/// <summary>
/// Used for reflection-based styles (<see cref="TooStringStyle.DebugView"/>, <see cref="TooStringStyle.JsonStringifier"/>, <see cref="TooStringStyle.CSharp"/>).
/// Choose between Json {"A":"B"}, DebugView { A = "B" }, or CSharp /*Type*/ new { A = "B" }
/// </summary>
public enum ReflectionStyle
{
    /// <summary>{"A":"B"} JSON style</summary>
    Json = 0,
    /// <summary>{ A = "B" } style</summary>
    DebugView = 1,

    /// <summary>
    /// /*Type*/ new { A = "B" } style - copy/pastable C#  anonymous objects
    /// with type names in comments
    /// </summary>
    CSharp = 2
}