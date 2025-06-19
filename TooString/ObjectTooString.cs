using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TooString;

/// <summary>
/// A class for creating human-readable strings for objects.
/// It differs from a serializer primarily in <b>not</b> being fail-fast.
/// If it can't stringify an object correctly, it will return a best-effort
/// rather than fail.
/// For the stringify methods offered, <see cref="TooStringHow"/>
/// </summary>
public static class ObjectTooString
{
    // ReSharper disable NotAccessedField.Local
    static int debugdepth;
    // ReSharper restore NotAccessedField.Local

    /// <summary>The string "null"</summary>
    public const string Null = "null";

    /// <summary>A Regex for recognising a valid C# Identifier</summary>
    public const string RegexCSharpIdentifier =
        @"@?[_\p{L}\p{Nl}][\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}\.]*";

    /// <summary>A Regex for recognising a valid C# Identifier</summary>
    public const string RegexTypeNameOrIdentifierWithCharsOnly =
        @"^[_\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}\.\`]+$";



    /// <summary>
    /// Stringifies a value using one of CallerArgumentExpressionAttribute, Json serialization,
    /// reflection, or ToString.
    /// <p>his method turns the parameters into a <see cref="TooStringOptions"/> and then
    /// calls <see cref="TooString{T}(T,TooStringOptions,string?)"/></p>
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="tooStringHow">The <em>method</em> by which the value is
    /// stringified. The default <see cref="TooStringHow.BestEffort"/> will
    /// use <see cref="TooStringHow.CallerArgument"/>
    /// if it holds more than just a parameter name, or <see cref="TooStringHow.Json"/>
    /// otherwise.
    /// </param>
    /// <param name="reflectionSerializationStyle">
    /// Only relevant when <paramref name="tooStringHow"/> is <see cref="TooStringHow.Reflection"/>.
    /// Choose between Json <c>{"A":"B"}</c> or Debug style <c>{A = "B"}</c>.
    /// Defaults to <see cref="ReflectionStyle.DebugView"/>
    /// </param>
    /// <param name="argumentExpression">
    /// This parameter should be ignored. It is automatically populated by the Compiler using
    /// <see cref="CallerArgumentExpressionAttribute"/>
    /// </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> according to the
    /// <paramref name="tooStringHow"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value,
                                      TooStringHow tooStringHow =
                                          TooStringHow.BestEffort,
                                      ReflectionStyle? reflectionSerializationStyle = null,
                                      [CallerArgumentExpression("value")]
                                      string? argumentExpression = null)
    {
        reflectionSerializationStyle ??= tooStringHow == TooStringHow.Json
                                             ? ReflectionStyle.Json
                                             : ReflectionStyle.DebugView;
        return TooString(value,
            TooStringOptions.Default with
            {
                PreferenceOrder = TooStringOptions
                    .Default.PreferenceOrder.Prepend(tooStringHow),
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
    /// <item>There are no options for <see cref="TooStringHow.ToString"/> or
    /// <see cref="TooStringHow.CallerArgument"/> or <see cref="TooStringHow.CSharpCode"/>
    /// </item>
    /// <item>Options for Json serialization are those for the
    /// <see cref="System.Text.Json.JsonSerializer"/> class.</item>
    /// <item>Options for Reflection serialization are the <see cref="BindingFlags"/> for what
    /// to stringify and <see cref="ReflectionOptions.MaxDepth"/> and whether
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
                   TooStringHow.BestEffort => CallerArgumentOrNextPreference(),
                   TooStringHow.CallerArgument => argumentExpression,
                   TooStringHow.Json => ToJson(value,tooStringOptions),
                   TooStringHow.Reflection => ToDebugViewString(value,tooStringOptions),
                   _ => value?.ToString()
               }
               ??
               Null;

        string CallerArgumentOrNextPreference()
        {
            foreach (var pref in tooStringOptions.PreferenceOrder.Skip(1))
            {
                //Only choose CallerArgument if we captured an expression
                if (pref== TooStringHow.CallerArgument
                    && argumentExpression is not null
                    && !Regex.IsMatch(argumentExpression,
                        RegexTypeNameOrIdentifierWithCharsOnly))
                    return argumentExpression;

                if (pref == TooStringHow.Reflection)
                    return ToDebugViewString(value, tooStringOptions);

                if (pref == TooStringHow.Json)
                    return ToJson(value, tooStringOptions);
            }
            return ToJson(value, tooStringOptions);
        }
    }

    /// <summary>
    /// Try to use <see cref="JsonSerializer"/> to serialize <paramref name="value"/>.
    /// If that fails — for instance for types in System.Reflection, and for System.Type itself,
    /// returns <see cref="ToDebugViewString{T}(T?,TooString.TooStringOptions)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="tooStringOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// JsonSerializer.Serialize(value,tooStringOptions.JsonOptions), unless that fails,
    /// in which case <see cref="ToDebugViewString{T}(T?,TooString.TooStringOptions)"/>.
    /// </returns>
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

    /// <summary>
    /// Serialize <paramref name="value"/> using System.Text.Json
    /// </summary>
    /// <param name="value"></param>
    /// <param name="writeIndented"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToJson<T>(this T? value, bool writeIndented = false)
        => ToJson(value,
            TooStringOptions.Default with
            {
                JsonOptions =
                TooStringOptions.DefaultJsonOptions.With(o
                    => o.WriteIndented = writeIndented)
            });

    /// <summary>
    /// Stringify value generating an output that is similar to Visual Studio's Debug View
    /// output style.
    /// <code>
    /// TypeName { A = 1, B = "A String", C = TypeName { D = 1.0 , E = 2025-06-01T14:00.1234456+01:00 } }
    /// </code>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToDebugViewString<T>(this T? value, TooStringOptions options)
        => ToDebugViewString(value, new OptionsWithState(0, options));

    static string ToDebugViewString<T>(T? value, OptionsWithState options)
    {
        Func<string?,string?> qname =
            options.ReflectionOptions.Style==ReflectionStyle.Json
                ? s => $"\"{s?.Replace("`","\\u0060").Replace("\"","\\\"")}\""
                : s => s;
        try
        {
            var (indent,outdent) =
                (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
                {
                    (ReflectionStyle.DebugView,_) => (" "," "),
                    (_,true) => ("\n",""),
                    (_,_) => ("","")
                };
            var delimiter = (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
                {
                    (ReflectionStyle.Json,true) => ",\n",
                    (ReflectionStyle.Json, _) =>",",
                    (ReflectionStyle.DebugView, _) => ",",
                    (_,_) => ","
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
                            ReflectionStyle.DebugView =>
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
            return value?.ToString() ?? Null;
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
            || type == typeof(Type)
            || type == typeof(DateTime)
            || type == typeof(DateOnly)
            || type == typeof(TimeOnly)
            || type == typeof(TimeSpan)
            ;
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

    /// <summary>
    /// Stringify <paramref name="value"/> using <see cref="TooStringHow.Reflection"/>
    /// and <see cref="ReflectionStyle.Json"/>
    /// and
    /// </summary>
    /// <param name="value"></param>
    /// <param name="style"></param>
    /// <param name="whichProperties"></param>
    /// <param name="indentedJson">Only relevant if <paramref name="style"/>
    ///     is <see cref="ReflectionStyle.Json"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToDebugViewString<T>(this T? value,
                                              ReflectionStyle style = ReflectionStyle.DebugView,
                                              BindingFlags whichProperties =
                                                  BindingFlags.Instance | BindingFlags.Public,
                                              bool indentedJson = false)
        => ToDebugViewString(
            value,
            new OptionsWithState(0,
                TooStringOptions.Default with
                {
                    JsonOptions = TooStringOptions.DefaultJsonOptions
                        .With(o=> o.WriteIndented=indentedJson),
                    ReflectionOptions = ReflectionOptions.Default with
                    {
                        Style = style,
                        WhichProperties = whichProperties,
                    },
                })
        );

    static string PrimitiveToShortReflectedString(object? value, OptionsWithState options)
    {
        Func<string?, string?> qstr =
                options.ReflectionOptions.Style == ReflectionStyle.Json
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
            options.ReflectionOptions.Style == ReflectionStyle.Json
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
            if (true.Equals(value)) return options.ReflectionOptions.Style==ReflectionStyle.DebugView ? "True" :"true";
            if (false.Equals(value)) return options.ReflectionOptions.Style==ReflectionStyle.DebugView ? "False" :"false";
            if (value.GetType().IsPrimitive) return value.ToString()!;
            if (value.GetType().IsArray) return "[]";
            if (value is Type t && t.IsAssignableTo(typeof(IEnumerable))) return "[]";
            if (value is Type t3 && (t3.Namespace?.StartsWith("System.Collections")??false)) return "[]";
            if(value is Type t2) return qstr( t2.FullName??$"{t2.Namespace}.{t2.Name}" );
            if (value is DateTime dateTime)
                return qstr(dateTime.ToString(options.ReflectionOptions.DateTimeFormat));
            if (value is DateOnly date)
                return qstr(date.ToString(options.ReflectionOptions.DateOnlyFormat));
            if (value is TimeOnly time)
                return qstr(time.ToString(options.ReflectionOptions.TimeOnlyFormat));
            if (value is TimeSpan span)
                return qstr(span.ToString(options.ReflectionOptions.TimeSpanFormat));
            return qstr(value.ToString() ?? Null);
        }
        catch
        {
            return qstr(value?.GetType().Name);
        }
    }
}