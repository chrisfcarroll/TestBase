namespace TooString;

/// <summary>
/// How <see cref="ObjectTooString.TooString{T}(T,StringifyAs)"/>
/// will stringify a value.
/// </summary>
public enum StringifyAs
{
    /// <summary>
    /// Stringify to C# anonymous object style: <c>new /*Type*/ { A = "B" }</c>
    /// Type information is included in inline comments.
    /// </summary>
    CSharp,

    /// <summary>Use
    /// <see cref="System.Text.Json.JsonSerializer.Serialize(object?,System.Type,System.Text.Json.JsonSerializerOptions?)"/>
    /// to serialize. If that fails, use <see cref="Json"/>.
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