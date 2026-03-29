using System.Text.Json;

namespace TooString;

/// <summary>
/// How <see cref="ObjectTooString.TooString{T}(T,TooStringStyle,TooStringOptions?)"/>
/// will stringify a value.
/// </summary>
public enum TooStringStyle
{
    /// <summary>
    /// Stringify to C# anonymous object style: <c>/*Type*/ new { A = "B" }</c>
    /// </summary>
    CSharp,

    /// <summary>Use
    /// <see cref="JsonSerializer.Serialize(object?,System.Type,System.Text.Json.JsonSerializerOptions?)"/>
    /// </summary>
    JsonSerializer,

    /// <summary>
    /// Stringify to JSON-style output: <c>{"A":"B"}</c>
    /// </summary>
    JsonStringifier,

    /// <summary>
    /// Stringify to ‘Debug View’ style: <c>{ A = B }</c>
    /// </summary>
    DebugView,
}