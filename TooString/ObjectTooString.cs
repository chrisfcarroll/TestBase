using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TooString;

public enum TooStringMethod
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
        return TooString(value,
            TooStringOptions.Default with
            {
                PreferenceOrder = TooStringOptions
                    .Default.PreferenceOrder.Prepend(tooStringMethod)
            }, 
            argumentExpression);
    }

    static string TooString<T>(T value,
                                TooStringOptions tooStringOptions,
                                [CallerArgumentExpression("value")]
                                string? argumentExpression = null)
    {
        return tooStringOptions.PreferenceOrder.FirstOrDefault() switch
               {
                   TooStringMethod.BestEffort => CallerArgumentOrNextPreference(),
                   TooStringMethod.CallerArgument => argumentExpression,
                   TooStringMethod.SystemTextJson => ToJson(value,tooStringOptions),
                   TooStringMethod.Reflection => ToReflectedString(value,tooStringOptions),
                   _ => value?.ToString()
               }
               ??
               Null;

        string CallerArgumentOrNextPreference()
        {
            foreach (var pref in tooStringOptions.PreferenceOrder.Skip(1))
            {
                //Only choose CallerArgument if we captured an expression
                if (pref== TooStringMethod.CallerArgument
                    && argumentExpression is not null
                    && !Regex.IsMatch(argumentExpression,
                        RegexTypeOrIdentifierNameCharsOnly))
                    return argumentExpression;

                if (pref == TooStringMethod.Reflection)
                    return ToReflectedString(value, tooStringOptions);

                if (pref == TooStringMethod.SystemTextJson)
                    return ToJson(value, tooStringOptions);
            }
            return ToJson(value, tooStringOptions);
        }
    }

    public static string ToJson<T>(this T? value, TooStringOptions tooStringOptions)
    {
        if (typeof(T).FullName == "System.Type" 
            || 
            typeof(T).FullName?.StartsWith("System.Reflection") is true)
        {
            return ToReflectedString(value, tooStringOptions);
        }

        try
        {
            return JsonSerializer.Serialize(value,tooStringOptions.JsonOptions);
        }
        catch
        {
            return ToReflectedString(value,tooStringOptions);
        }
    }

    public static string ToJson<T>(this T? value, bool writeIndented = false)
        => ToJson(value,
            TooStringOptions.Default with
            {
                JsonOptions =
                TooStringOptions.DefaultJsonOptions.With(o
                    => o.WriteIndented = writeIndented)
            });

    static string ToReflectedString<T>(T? value, TooStringOptions options)
        => ToReflectedString(value, new OptionsWithState(0, options));

    static string ToReflectedString<T>(T? value, OptionsWithState options)
    {
        Func<string?,string> q = 
            options.ReflectionOptions.Style==SerializationStyle.Json 
                ? s => $"\"{s?.Replace("`","\\u0060").Replace("\"","\\\"")}\"" 
                : s => s;
        
        var indent = 
            (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
            {
                (SerializationStyle.CSharp,_)=>" ",
                (_,true) => "\n",
                (_,_)=>"",
            };

        if (value is null) return q(null);
        if (value is string svalue) return q(svalue);
        
        try
        {
            return "{" 
                   + indent 
                   + string.Join("," + indent,
                       value.GetType()
                           .GetTypeInfo()
                           .GetProperties(options.ReflectionOptions.WhichProperties)
                           .Where(p=>p.CanRead)
                           .Select(
                               p => options.ReflectionOptions.Style switch
                               {
                                   SerializationStyle.CSharp => $"{p.Name}:{TryGetValue(p)}",
                                   _ => $"{q(p.Name)}:{TryGetValue(p)}",
                               })
                   )
                   + "}";
        }
        catch { return value.ToString()??Null; }
        
        string TryGetValue(PropertyInfo p)
        {
            try
            {
                if (p.PropertyType.IsPrimitive
                    || p.PropertyType == typeof(string)
                    || (options.Depth > options.ReflectionOptions.MaxDepth)
                    || p.PropertyType == typeof(Type))
                    return PrimitiveToShortReflectedString(
                        p.GetValue(value) ?? "null",
                        options);
                else
                    return ToReflectedString(
                        p.GetValue(value) ?? "null",
                        options with { Depth = options.Depth + 1 });
            }
            catch
            {
                return "\"cantretrievevalue\"";
            }
        }
    }

    public static string ToReflectedString<T>(this T? value,
                                              bool writeIndented = false,
                                              bool quotePropertyNames = false,
                                              BindingFlags whichProperties =
                                                  BindingFlags.Instance |
                                                  BindingFlags.Public)
        => ToReflectedString(
            value,
            new OptionsWithState(0,
                TooStringOptions.Default with
                {
                    JsonOptions = TooStringOptions.DefaultJsonOptions
                        .With(o=> o.WriteIndented=writeIndented),
                    ReflectionOptions = ReflectionSerializerOptions.Default with
                    {
                        Style = quotePropertyNames 
                            ? SerializationStyle.Json 
                            : SerializationStyle.CSharp
                    }
                })
        );

    static string PrimitiveToShortReflectedString(object? value, OptionsWithState options)
    {
        Func<string,string> q = 
            options.ReflectionOptions.Style==SerializationStyle.Json 
                ? s => string.Format("\"{0}\"", s?
                        .Replace("`", "\\u0060")
                        .Replace("\\", "\\\\")
                        .Replace("\t", "\\\t")
                        .Replace("\n", "\\\n")
                        .Replace("\r", "\\\r")
                        .Replace("\"", "\\\"")
                    )
                : s => s;
        
        if(value is null) return q(Null);
        if(value is string s) return q(s);
        if (value.GetType().IsTypeDefinition) return q( value.ToString()! );
        if (value.GetType().IsEnum) return q(value.ToString()!);
        if (value.GetType().IsPrimitive) return q(value.ToString()!);
        if (value.GetType().IsArray) return "[]";
        if (value is Type t && t.IsAssignableTo(typeof(IEnumerable))) return "[]";
        if (value is Type t3 && t3.Namespace.StartsWith("System.Collections")) return "[]";
        if(value is Type t2) return q( t2.FullName??$"{t2.Namespace}.{t2.Name}" );
        
        try{ return q( value.ToString()??Null );}
        catch{ return q("cantretrievevalue");}
    }
}