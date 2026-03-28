using System.Runtime.CompilerServices;
using System.Text.Json;

namespace TooString;

/// <summary>
/// The unified style for <see cref="ObjectTooString.TooString{T}(T,TooStringStyle,TooStringOptions?,string?)"/>
/// to stringify a value. Combines the serialization method and output style into a single choice.
/// </summary>
public enum TooStringStyle
{
    /// <summary>If <see cref="CallerArgument"/> returns more than just a parameter
    /// name, then use it.
    /// Otherwise use <see cref="System.Text.Json.JsonSerializer"/>
    /// </summary>
    BestEffort = 0,

    /// <summary>Use
    /// <see cref="CallerArgumentExpressionAttribute"/> available on Net5.0 and above
    /// </summary>
    CallerArgument,

    /// <summary>Use
    /// <see cref="JsonSerializer.Serialize(object?,System.Type,System.Text.Json.JsonSerializerOptions?)"/>
    /// </summary>
    JsonSerializer,

    /// <summary>
    /// Use Reflection to stringify with JSON-style output: <c>{"A":"B"}</c>
    /// </summary>
    JsonStringifier,

    /// <summary>
    /// Use Reflection to stringify with Debug View style: <c>{ A = "B" }</c>
    /// </summary>
    DebugView,

    /// <summary>
    /// Use Reflection to stringify with C# anonymous object style: <c>/*Type*/ new { A = "B" }</c>
    /// </summary>
    CSharp,

    /// <summary>Use
    /// <c>value?.ToString()??<see cref="ObjectTooString.Null"/></c>
    /// </summary>
    ToString,
}

/// <summary>
/// Extension methods for <see cref="TooStringStyle"/>
/// </summary>
public static class TooStringStyleExtensions
{
    /// <summary>Whether this style uses reflection-based serialization</summary>
    public static bool IsReflection(this TooStringStyle style) =>
        style is TooStringStyle.JsonStringifier or TooStringStyle.DebugView or TooStringStyle.CSharp;

    /// <summary>Convert to the internal <see cref="ReflectionStyle"/> used by <see cref="ReflectionOptions"/></summary>
    public static ReflectionStyle ToReflectionStyle(this TooStringStyle style) => style switch
    {
        TooStringStyle.JsonStringifier => ReflectionStyle.Json,
        TooStringStyle.CSharp => ReflectionStyle.CSharp,
        _ => ReflectionStyle.DebugView
    };
}
