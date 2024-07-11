using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TooString;

public static class ObjectTooString
{
    static int debugdepth;
    public const string Null = "null";

    public const string RegexCSharpIdentifier =
        @"@?[_\p{L}\p{Nl}][\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}\.]*";
    
    public const string RegexTypeNameOrIdentifierWithCharsOnly =
        @"^[_\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}\.\`]+$";
    
    

    /// <summary>
    /// Stringifies a value using one of CallerArgumentExpressionAttribute, Json serialization,
    /// reflection, or ToString.
    /// <p>his method turns the parameters into a <see cref="TooStringOptions"/> and then
    /// calls <see cref="TooString{T}(T,TooStringOptions,string?)"/></p>
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="tooStringStyle">The <em>method</em> by which the value is
    /// stringified. The default <see cref="TooStringStyle.BestEffort"/> will
    /// use <see cref="TooStringStyle.CallerArgument"/>
    /// if it holds more than just a parameter name, or <see cref="TooStringStyle.Json"/>
    /// otherwise.
    /// </param>
    /// <param name="reflectionSerializationStyle">
    /// Only relevant when <paramref name="tooStringStyle"/> is <see cref="TooStringStyle.Reflection"/>.
    /// Choose between Json <c>{"A":"B"}</c> or Debug style <c>{A = "B"}</c>.
    /// Defaults to <see cref="SerializationStyle.DebugDisplay"/>
    /// </param>
    /// <param name="argumentExpression">
    /// This parameter should be ignored. It is automatically populated by the Compiler using 
    /// <see cref="CallerArgumentExpressionAttribute"/>
    /// </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> according to the
    /// <paramref name="tooStringStyle"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value,
                                      TooStringStyle tooStringStyle =
                                          TooStringStyle.BestEffort,
                                      SerializationStyle? reflectionSerializationStyle = null,
                                      [CallerArgumentExpression("value")]
                                      string? argumentExpression = null)
    {
        reflectionSerializationStyle ??= tooStringStyle == TooStringStyle.Json
                                             ? SerializationStyle.Json
                                             : SerializationStyle.DebugDisplay;
        return TooString(value,
            TooStringOptions.Default with
            {
                PreferenceOrder = TooStringOptions
                    .Default.PreferenceOrder.Prepend(tooStringStyle),
                ReflectionOptions = TooStringOptions
                    .Default.ReflectionOptions with {Style = reflectionSerializationStyle.Value}
            },
            argumentExpression);
    }

    /// <summary>
    /// Stringifies a value using one of CallerArgumentExpressionAttribute, Json serialization,
    /// reflection, or ToString
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="tooStringOptions">Configure the method used, and the options
    /// relevant to the method used.
    /// <list type="bullet">
    /// <item>There are no options for <see cref="TooStringStyle.ToString"/> or
    /// <see cref="TooStringStyle.CallerArgument"/> or <see cref="TooStringStyle.CSharpCode"/>
    /// </item>
    /// <item>Options for Json serialization are those for the
    /// <see cref="System.Text.Json.JsonSerializer"/> class.</item>
    /// <item>Options for Reflection serialization are the <see cref="BindingFlags"/> for what
    /// to serialize and <see cref="ReflectionSerializerOptions.MaxDepth"/> and whether
    /// to output Json or Debug style.</item>
    /// </list>
    /// See <see cref="TooStringOptions"/> for details of what can be configured.
    /// The easy way to change options may be to use <see cref="TooStringOptions.Default"/>
    /// with a <c>with</c> expression: <c>TooStringOptions.Default with { ... }</c>
    /// </param>
    /// <param name="argumentExpression">
    /// This parameter should be ignored. It is automatically populated by the Compiler using 
    /// <see cref="CallerArgumentExpressionAttribute"/>
    /// </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> according to the
    /// <paramref name="tooStringOptions"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value,
                                TooStringOptions tooStringOptions,
                                [CallerArgumentExpression("value")]
                                string? argumentExpression = null)
    {
        return tooStringOptions.PreferenceOrder.FirstOrDefault() switch
               {
                   TooStringStyle.BestEffort => CallerArgumentOrNextPreference(),
                   TooStringStyle.CallerArgument => argumentExpression,
                   TooStringStyle.Json => ToJson(value,tooStringOptions),
                   TooStringStyle.Reflection => ToDebugViewString(value,tooStringOptions),
                   _ => value?.ToString()
               }
               ??
               Null;

        string CallerArgumentOrNextPreference()
        {
            foreach (var pref in tooStringOptions.PreferenceOrder.Skip(1))
            {
                //Only choose CallerArgument if we captured an expression
                if (pref== TooStringStyle.CallerArgument
                    && argumentExpression is not null
                    && !Regex.IsMatch(argumentExpression,
                        RegexTypeNameOrIdentifierWithCharsOnly))
                    return argumentExpression;

                if (pref == TooStringStyle.Reflection)
                    return ToDebugViewString(value, tooStringOptions);

                if (pref == TooStringStyle.Json)
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
                    (SerializationStyle.DebugDisplay,_) => (" "," "),
                    (_,true) => ("\n",""),
                    (_,_) => ("","")
                };
            var delimiter = (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
                {
                    (SerializationStyle.Json,true) => ",\n",
                    (SerializationStyle.Json, _) =>",",
                    (SerializationStyle.DebugDisplay, _) => ",",
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
                            SerializationStyle.DebugDisplay =>
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
                            : SerializationStyle.DebugDisplay
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
            if (true.Equals(value)) return options.ReflectionOptions.Style==SerializationStyle.DebugDisplay ? "True" :"true";
            if (false.Equals(value)) return options.ReflectionOptions.Style==SerializationStyle.DebugDisplay ? "False" :"false";
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