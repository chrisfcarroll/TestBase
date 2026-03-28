namespace TooString;

/// <summary>
/// Used for reflection-based styles (<see cref="TooStringStyle.DebugView"/>, <see cref="TooStringStyle.JsonStringifier"/>, <see cref="TooStringStyle.CSharp"/>).
/// Choose between JSON {"A":"B"}, CSharp /*Type*/ new { A = "B" } or DebugView { A = B }
/// </summary>
public enum ReflectionStyle
{
    /// <summary>Prints JSON <c>{"A":"B"}</c> output</summary>
    Json,

    /// <summary>
    /// Prints C# anonymous object output,  <c>/*Type*/ new { A = "B" }</c>,
    /// with type names commented out.
    /// </summary>
    CSharp,

    /// <summary>Prints ‘DebugView’ output { A = B }.</summary>
    DebugView,
}