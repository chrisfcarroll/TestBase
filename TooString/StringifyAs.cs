namespace TooString;

/// <summary>
/// How <see cref="ObjectTooString.TooString{T}(T,StringifyAs,TooStringOptions?)"/>
/// will stringify a value.
/// </summary>
public enum StringifyAs
{
    /// <summary>
    /// Stringify to C# anonymous object style: <c>/*Type*/ new { A = "B" }</c>
    /// </summary>
    CSharp,

    /// <summary>Use
    /// <see cref="System.Text.Json.JsonSerializer.Serialize(object?,System.Type,System.Text.Json.JsonSerializerOptions?)"/>
    /// to serialize.
    /// </summary>
    STJson,

    /// <summary>
    /// Stringify to JSON-style output: <c>{"A":"B"}</c>
    /// </summary>
    Json,

    /// <summary>
    /// Stringify to ‘Debug View’ style: <c>{ A = B }</c>
    /// </summary>
    DebugView,
}