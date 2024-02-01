using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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
    static int debugdepth;
    public const string Null = "null";

    public const string RegexCSharpIdentifier =
        @"@?[_\p{L}\p{Nl}][\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}\.]*";
    
    public const string RegexTypeNameOrIdentifierWithCharsOnly =
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
                   TooStringMethod.Reflection => ToDebugViewString(value,tooStringOptions),
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
                        RegexTypeNameOrIdentifierWithCharsOnly))
                    return argumentExpression;

                if (pref == TooStringMethod.Reflection)
                    return ToDebugViewString(value, tooStringOptions);

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
            return ToDebugViewString(value, tooStringOptions);
        }

        try
        {
            return JsonSerializer.Serialize(value,tooStringOptions.JsonOptions);
        }
        catch
        {
            return ToDebugViewString(value,tooStringOptions);
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

    public static string ToDebugViewString<T>(this T? value, TooStringOptions options)
        => ToDebugViewString(value, new OptionsWithState(0, options));

    static string ToDebugViewString<T>(T? value, OptionsWithState options)
    {
        Func<string?,string> qname = 
            options.ReflectionOptions.Style==SerializationStyle.Json 
                ? s => $"\"{s?.Replace("`","\\u0060").Replace("\"","\\\"")}\"" 
                : s => s;
        try
        {
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

            if (IsScalarish(value.GetType()) ||
                options.Depth > options.ReflectionOptions.MaxDepth)
            {
                return PrimitiveToShortReflectedString(value, options);
            }
            else if (value is ITuple valueTuple)
            {
                return DelimitTuple(valueTuple, options);
            }
            else
            {
                var props = value.GetType()
                    .GetTypeInfo()
                    .GetProperties(options.ReflectionOptions.WhichProperties)
                    .Where(p => p.CanRead).ToList();
                debugdepth = options.Depth;
                var contents = props
                    .Select(
                        p => options.ReflectionOptions.Style switch
                        {
                            SerializationStyle.DotNetDebug =>
                                $"{p.Name} = {TryGetValue(p)}",
                            _ => 
                                $"{qname(p.Name)}:{TryGetValue(p)}",
                        }).ToList();
                return "{" +
                       indent +
                       string.Join(delimiter + indent, contents) 
                       +
                       outdent +
                       "}";
            }
        }
        catch
        {
            return value.ToString()??Null;
        }
        
        string TryGetValue(PropertyInfo p)
        {
            try
            {
                if (IsScalarish(p.PropertyType) 
                    || options.Depth > options.ReflectionOptions.MaxDepth)
                    return PrimitiveToShortReflectedString(
                        p.GetValue(value) ?? "null", options);
                else if(p.GetIndexParameters().Any())
                    return qname(p.PropertyType.Name);
                else
                    return ToDebugViewString(
                        p.GetValue(value) ?? "null",
                        options with { Depth = options.Depth + 1 });
            }
            catch
            {
                return qname(p.PropertyType.Name);
            }
        }

        bool IsScalarish(Type type) => 
                        type.IsPrimitive
                     || type.IsEnum
                     || type == typeof(string)
                     || type == typeof(Type);
    }

    static string DelimitTuple<T>(T value, OptionsWithState options) where T : ITuple
    {
        var b = new StringBuilder("(");
        for (int i = 1; i < (value as ITuple).Length; i++)
        {
            //b.Append("item");
            //b.Append(i);
            //b.Append(" = ");
            b.Append(ToDebugViewString(value[ i -1 ],
                options with { Depth = options.Depth + 1 }));
            b.Append(", ");
        }
        b.Append(ToDebugViewString(value[ value.Length ],
                options with { Depth = options.Depth + 1 }));
        b.Append(')');
        return b.ToString();
    }

    public static string ToDebugViewString<T>(this T? value,
                                                bool writeIndented = false,
                                                bool quotePropertyNames = false,
                                                BindingFlags whichProperties =
                                                    BindingFlags.Instance |
                                                    BindingFlags.Public)
        => ToDebugViewString(
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
        
        try
        {
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
            return qstr(value.ToString() ?? Null);
        }
        catch
        {
            return qstr(value.GetType().Name);
        }
    }
}