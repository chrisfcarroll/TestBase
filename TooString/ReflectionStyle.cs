namespace TooString;

/// <summary>
/// Only used for <see cref="TooStringHow.Reflection"/>.
/// Choose between Json {"A":"B"} or DebugView { A = "B" }
/// </summary>
public enum ReflectionStyle
{
    /// <summary>{"A":"B"} style</summary>
    Json = 0,
    /// <summary>{ A = "B" } style</summary>
    DebugView = 1
}