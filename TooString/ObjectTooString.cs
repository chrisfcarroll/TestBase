using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    const string RegexVariableName =
        @"@?[_\p{L}\p{Nl}][\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}\.]*";
    
    const string RegexTypeOrIdentifierNameCharsOnly =
        @"^[_\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}\.\`]+$";
    
    

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
                   TooStringMethod.SystemTextJson => ToJson(value),
                   TooStringMethod.Reflection => TooReflectedString(value),

                   _ => value?.ToString()
               }
               ??
               Null;

        string ChooseExpressionOrJson()
        {
            return (Regex.IsMatch(argumentExpression, RegexTypeOrIdentifierNameCharsOnly))
                ? value.ToJson()
                : argumentExpression;
        }
    }

    public static string ToJson<T>(this T? value, bool writeIndented=false)
    {
        if (typeof(T).FullName == "System.Type" 
            || 
            typeof(T).FullName?.StartsWith("System.Reflection") is true)
        {
            return TooReflectedString(value, 
                        writeIndented:writeIndented,
                        quotePropertyNames:true);
        }

        try
        {
            return JsonSerializer.Serialize(
                value,
                new JsonSerializerOptions(JsonSerializerDefaults.General)
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    MaxDepth = 99,
                    WriteIndented = writeIndented,
                }
            );
        }
        catch
        {
            return TooReflectedString(value, 
                        writeIndented:writeIndented,
                        quotePropertyNames:true);
        }
    }

    public static string TooReflectedString<T>(this T? obj,
                                               bool writeIndented = false,
                                               bool quotePropertyNames = false,
                                               BindingFlags whichProperties =
                                                   BindingFlags.Instance |
                                                   BindingFlags.Public)
    {
        if (obj is null) return Null;
        var delimiter = writeIndented ? ",\n" : ",";
        Func<string,string> q = quotePropertyNames 
            ? s => $"\"{s?.Replace("`","\\u0060")}\"" 
            : s => s;
        try
        {
            return "{" + 
                    q("Type") + $":\"{typeof(T).FullName}\"" + 
                   delimiter +
                   string.Join(",",
                       obj.GetType()
                           .GetTypeInfo()
                           .GetProperties(whichProperties)
                           .Where(p=>p.CanRead)
                           .Select(p => $"{q(p.Name)}:\"{TryGetValue(p).ToString().Replace("`","\\u0060")}\""))
                   + "}";
        }
        catch
        {
            return obj.ToString()??Null;
        }

        object? TryGetValue(PropertyInfo p)
        { 
            try{ return p.GetValue(obj)??"null" ;}
            catch{return "cantretrieve";}
        }
    }
}