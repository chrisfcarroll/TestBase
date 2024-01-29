using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
                                      SerializationStyle? serializationStyle = null,
                                      [CallerArgumentExpression("value")]
                                      string? argumentExpression = null)
    {
        serializationStyle ??= tooStringMethod == TooStringMethod.SystemTextJson
                                   ? SerializationStyle.Json
                                   : SerializationStyle.DotNetDebug;
        return TooString(value,
            TooStringOptions.Default with
            {
                PreferenceOrder = TooStringOptions
                    .Default.PreferenceOrder.Prepend(tooStringMethod),
                ReflectionOptions = TooStringOptions
                    .Default.ReflectionOptions with {Style = serializationStyle.Value}
            },
            argumentExpression);
    }

    public static string TooString<T>(this T value,
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
        Func<string?,string> qname = 
            options.ReflectionOptions.Style==SerializationStyle.Json 
                ? s => $"\"{s?.Replace("`","\\u0060").Replace("\"","\\\"")}\"" 
                : s => s;
        
        var (indent,outdent) = 
            (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
            {
                (SerializationStyle.DotNetDebug,_) => (" "," "),
                (_,true) => ("\n",""),
                (_,_) => ("","")
            };
        var delimiter = (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
            {
                (SerializationStyle.Json,true) => ",\n",
                (SerializationStyle.Json, _) =>",",
                (SerializationStyle.DotNetDebug, _) => ",",
            };
        
        if (value is null) return qname(null);
        if (value is string svalue) return qname(svalue);
        
        try
        {
            if (IsScalarish(value.GetType()) || options.Depth > options.ReflectionOptions.MaxDepth)
            {
                return PrimitiveToShortReflectedString(value, options);
            }
            else
            {
                return "{" +
                       indent +
                       string.Join(delimiter + indent,
                                   value.GetType()
                                        .GetTypeInfo()
                                        .GetProperties(options.ReflectionOptions.WhichProperties)
                                        .Where(p => p.CanRead)
                                        .Select(
                                            p => options.ReflectionOptions.Style switch
                                            {
                                                SerializationStyle.DotNetDebug =>
                                                    $"{p.Name} = {TryGetValue(p)}",
                                                _ => $"{qname(p.Name)}:{TryGetValue(p)}",
                                            })
                       ) +
                       outdent +
                       "}";
            }
        }
        catch { return value.ToString()??Null; }
        
        string TryGetValue(PropertyInfo p)
        {
            try
            {
                if (IsScalarish(p.PropertyType) 
                    || options.Depth > options.ReflectionOptions.MaxDepth)
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

        bool IsScalarish(Type type) => 
                        type.IsPrimitive
                     || type.IsEnum
                     || type == typeof(string)
                     || type == typeof(Type);
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
                            : SerializationStyle.DotNetDebug
                    }
                })
        );

    static string PrimitiveToShortReflectedString(object? value, OptionsWithState options)
    {
        Func<string, string> qstr =
                options.ReflectionOptions.Style == SerializationStyle.Json
                    ? s => string.Format("\"{0}\"", s?
                                                    .Replace("\\", "\\\\")
                                                    .Replace("`", "\\u0060")
                                                    .Replace("\t", "\\\t")
                                                    .Replace("\n", "\\\n")
                                                    .Replace("\r", "\\\r")
                                                    .Replace("\"", "\\\"")
                      )
                    : s => s
            ;
        Func<string, string> qEnum =
            options.ReflectionOptions.Style == SerializationStyle.Json
                ? s => string.Format("\"{0}\"", s?
                                                .Replace("\\", "\\\\")
                                                .Replace("`", "\\u0060")
                                                .Replace("\t", "\\\t")
                                                .Replace("\n", "\\\n")
                                                .Replace("\r", "\\\r")
                                                .Replace("\"", "\\\"")
                  )
                : s => s;
        
        if(value is null) return qstr(Null);
        if(value is string s) return qstr(s);
        if (value.GetType().IsEnum) return qEnum(value.ToString()!);
        if (true.Equals(value)) return options.ReflectionOptions.Style==SerializationStyle.DotNetDebug ? "True" :"true";
        if (false.Equals(value)) return options.ReflectionOptions.Style==SerializationStyle.DotNetDebug ? "False" :"false";
        if (value.GetType().IsPrimitive) return value.ToString()!;
        if (value.GetType().IsArray) return "[]";
        if (value is Type t && t.IsAssignableTo(typeof(IEnumerable))) return "[]";
        if (value is Type t3 && (t3.Namespace?.StartsWith("System.Collections")??false)) return "[]";
        if(value is Type t2) return qstr( t2.FullName??$"{t2.Namespace}.{t2.Name}" );

        try
        {
            return qstr(value.ToString() ?? Null);
        }
        catch
        {
            return qstr("cantretrievevalue");
        }
    }
}