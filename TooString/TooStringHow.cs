using System.Runtime.CompilerServices;
using System.Text.Json;

namespace TooString;

/// <summary>
/// The method by which <see cref="ObjectTooString.TooString{T}(T,TooStringHow,System.Nullable{ReflectionStyle},string?)"/>
/// will stringify a value. One of CallerArgument,Json,Reflection,ToString or BestEffort
/// </summary>
public enum TooStringHow
{
    /// <summary>If <see cref="CallerArgument"/> returns more than just a parameter
    /// name, then use it.
    /// Otherwise use <see cref="JsonSerializer"/> 
    /// </summary>
    BestEffort = 0,

    /// <summary>Use
    /// <see cref="CallerArgumentExpressionAttribute"/> available on Net5.0 and above
    /// </summary>
    CallerArgument,
    
    /// <summary>Use
    /// <see cref="CallerArgumentExpressionAttribute"/> available on Net5.0 and above
    /// </summary>
    CSharpCode=CallerArgument,

    /// <summary>Use
    /// <see cref="JsonSerializer.Serialize(object?,System.Type,System.Text.Json.JsonSerializerOptions?)"/>
    /// </summary>
    Json,

    /// <summary>Use
    /// <c>value.GetType().GetTypeInfo()
    ///     .GetProperties(whichProperties)
    ///     .Select(p => $"{p.Name}={p.GetValue(obj)}"))</c>
    /// similar to typical debugger watch expression display.
    /// </summary>
    Reflection,

    /// <summary>Use
    /// <c>value?.ToString()??<see cref="ObjectTooString.Null"/></c>
    /// </summary>
    ToString,
}