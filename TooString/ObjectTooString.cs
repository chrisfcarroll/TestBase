using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TooString;

public enum TooStringMethod
{
    /// <summary>If <see cref="ArgumentExpression"/> returns more than just a parameter
    /// name, then use it.
    /// Otherwise use <see cref="JsonSerializer"/> 
    /// </summary>
    BestEffort = 0,

    /// <summary>Use
    /// <see cref="CallerArgumentExpressionAttribute"/> available on Net5.0 and above
    /// </summary>
    ArgumentExpression,

    /// <summary>Use
    /// <see cref="JsonSerializer.Serialize(object?,System.Type,System.Text.Json.JsonSerializerOptions?)"/>
    /// </summary>
    SystemTextJson,

    /// <summary>Use
    /// <c>value.GetType().GetTypeInfo()
    ///     .GetProperties(whichProperties)
    ///     .Select(p => $"{p.Name}={p.GetValue(obj)}"))</c>
    /// </summary>
    Reflection,

    /// <summary>Use
    /// <c>value?.ToString()??<see cref="ObjectTooString.Null"/></c>
    /// </summary>
    ToString,
}

public static class ObjectTooString
{
    public const string Null = "null";

    public const string RegexVariableName =
        @"@?[_\p{L}\p{Nl}][\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]*";

    public static string TooString<T>(this T value,
                                      TooStringMethod tooStringMethod =
                                          TooStringMethod.BestEffort,
                                      [CallerArgumentExpression("value")]
                                      string? argumentExpression = null)
    {
        return tooStringMethod switch
               {
                   TooStringMethod.BestEffort => ChooseExpressionOrJson(),
                   TooStringMethod.ArgumentExpression => argumentExpression,
                   TooStringMethod.Reflection => TooReflectedString(value),

                   _ => value?.ToString()
               }
               ??
               Null;

        string ChooseExpressionOrJson()
        {
            return (Regex.IsMatch(argumentExpression, RegexVariableName))
                ? value.ToJson()
                : argumentExpression;
        }
    }

    public static string ToJson(this object value)
    {
        return JsonSerializer.Serialize(value);
    }

    public static string TooReflectedString<T>(this T? obj,
                                               BindingFlags whichProperties =
                                                   BindingFlags.Instance |
                                                   BindingFlags.Public)
    {
        if (obj is null) return Null;
        try
        {
            return "{ Type=" + nameof(T) + ", " +
                   string.Join(", ",
                       obj.GetType()
                           .GetTypeInfo()
                           .GetProperties(whichProperties)
                           .Select(p => $"{p.Name}={p.GetValue(obj)}"))
                   + "}";
        }
        catch (Exception e)
        {
            return obj.ToString();
        }
    }
}