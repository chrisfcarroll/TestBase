namespace TooString;

/// <summary>
/// Only used for <see cref="TooStringStyle.Reflection"/>.
/// Choose between Json {"A":"B"} or DebugDisplay { A = "B" }
/// </summary>
public enum SerializationStyle
{
    /// <summary>{"A":"B"} style</summary>
    Json = 0,
    /// <summary>{ A = "B" } style</summary>
    DebugDisplay = 1
}