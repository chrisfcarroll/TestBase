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
    /// Stringifies a value use <see cref="TooStringHow.Reflection"/>, with
    /// style chosen by <paramref name="reflectionStyle"/>
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="reflectionStyle"></param>
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
                                      ReflectionStyle reflectionStyle,
                                      [CallerArgumentExpression("value")]
                                      string? argumentExpression = null)
    {
        var options = TooStringOptions.Default with
        {
            Fallbacks = [TooStringHow.Reflection],
            ReflectionOptions = TooStringOptions.Default.ReflectionOptions
                                    with { Style = reflectionStyle }
        };
        return TooString(value,options,argumentExpression);
    }

    /// <summary>
    /// Stringifies a value using one of CallerArgumentExpressionAttribute, Json serialization,
    /// reflection, or ToString.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="tooStringHow">The <em>method</em> by which the value is
    /// stringified. The default <see cref="TooStringHow.BestEffort"/> will
    /// use <see cref="TooStringHow.CallerArgument"/>
    /// if <paramref name="argumentExpression"/> holds more than just a parameter name,
    /// or <see cref="TooStringHow.Json"/> otherwise.
    /// </param>
    /// <param name="options"></param>
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
                                      TooStringHow tooStringHow = TooStringHow.BestEffort,
                                      TooStringOptions? options = null,
                                      [CallerArgumentExpression("value")]
                                      string? argumentExpression = null)
    {
        options ??= (tooStringHow) switch
                    {
                        TooStringHow.Reflection => TooStringOptions.ForReflection(),
                        TooStringHow.Json => TooStringOptions.ForJson(),
                        _ => TooStringOptions.Default
                    };
        options = options with
                    {
                        Fallbacks = TooStringOptions.Default.Fallbacks.Prepend(tooStringHow),
                    };
        return TooString(value,options,argumentExpression);
    }

    /// <summary>
    /// Stringifies a value using one of CallerArgumentExpressionAttribute, Json serialization,
    /// reflection, or ToString.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="maxDepth">
    /// How deep in nested object structures to descend before stopping further serialization.
    /// Properties at this level will be typically be stringified with their <see cref="object.ToString"/>
    /// method.
    /// </param>
    /// <param name="maxLength">
    /// How many items of an <see cref="IEnumerable{T}"/> property to stringify.
    /// Further items will simply be omitted.
    /// </param>
    /// <param name="style">
    /// Choose between <see cref="ReflectionStyle.Json"/> and
    /// <see cref="ReflectionStyle.DebugView"/>
    /// </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> according to the
    /// <paramref name="style"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value,
                                      int maxDepth,
                                      int maxLength = 9,
                                      ReflectionStyle style = ReflectionStyle.Json)
    {
        var options = TooStringOptions.Default with
        {
            Fallbacks = TooStringOptions.Default.Fallbacks.Prepend(TooStringHow.Reflection),
            ReflectionOptions = TooStringOptions.Default.ReflectionOptions
                with
                {
                    Style = style,
                    MaxDepth = maxDepth,
                    MaxLength = maxLength
                }
        };
        return TooString(value,options);
    }

    /// <summary>
    /// Stringifies a value using reflection, with the options specified.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="reflectionOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> built by recursively
    /// reflecting the properties of <paramref name="value"/> using the
    /// <paramref name="reflectionOptions"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value, ReflectionOptions reflectionOptions)
    {
        var options = TooStringOptions.Default with
        {
            Fallbacks = TooStringOptions.Default.Fallbacks.Prepend(TooStringHow.Reflection),
            ReflectionOptions = reflectionOptions
        };
        return TooString(value,options);
    }

    /// <summary>
    /// Stringifies a value by first trying System.Text.Json.JsonSerializer (unless
    /// value is an <see cref="ITuple"/>) or if that fails, by reflection,
    /// with the options specified.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="jsonOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// Either a Json serialization of <paramref name="value"/>, or if that fails
    /// a string representation of <paramref name="value"/> built by recursively
    /// reflecting the properties of <paramref name="value"/> using the
    /// <paramref name="jsonOptions"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value, JsonSerializerOptions jsonOptions)
    {
        var options = TooStringOptions.ForJson() with
        {
            JsonOptions = jsonOptions,
        };
        return TooString(value,options);
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
        return tooStringOptions.Fallbacks.FirstOrDefault() switch
               {
                   TooStringHow.BestEffort => CallerArgumentOrNextPreference(),
                   TooStringHow.CallerArgument => argumentExpression,
                   TooStringHow.Json => ToJson(value,tooStringOptions),
                   TooStringHow.Reflection => BuildReflectedString(value, new OptionsWithState(0, tooStringOptions)),
                   _ => value?.ToString()
               }
               ??
               Null;

        string CallerArgumentOrNextPreference()
        {
            foreach (var pref in tooStringOptions.Fallbacks.Skip(1))
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
    /// returns <see cref="ToDebugViewString{T}(T?,TooString.TooStringOptions?)"/>
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
            typeof(T).FullName?.StartsWith("System.Reflection") is true
            ||
            (value is ITuple && !tooStringOptions.JsonOptions.IncludeFields))
        {
            return ToDebugViewString(value, tooStringOptions with {ReflectionOptions = tooStringOptions.ReflectionOptions with {Style = ReflectionStyle.Json}});
        }

        try
        {
            return JsonSerializer.Serialize(value,tooStringOptions.JsonOptions);
        }
        catch
        {
            return ToDebugViewString(value,TooStringOptions.ForJson() with {JsonOptions = tooStringOptions.JsonOptions});;
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
        => BuildReflectedString(value, new OptionsWithState(0, options));

    static string BuildReflectedString<T>(T? value, OptionsWithState options)
    {
        if (options.Depth >= options.ReflectionOptions.MaxDepth)
        {
            return ScalarishToShortReflectedString(value,options);
        }

        Func<string,string> qname =
            options.ReflectionOptions.Style==ReflectionStyle.Json
                ? s => $"\"{s?.Replace("`","\\u0060").Replace("\"","\\\"")}\""
                : s => s;
        try
        {
            var indent=
                (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
                {
                    (_,true) => CrLfSpaces400.AsSpan().Slice(0, 4 + options.Depth * 2),
                    (ReflectionStyle.Json,false) => "".AsSpan(),
                    (ReflectionStyle.DebugView,false) => " ".AsSpan(),
                    (_,_) => " ".AsSpan()
                };
            var outdent =
                (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
                {
                    (_,true) => CrLfSpaces400.AsSpan().Slice(0, 2 + options.Depth * 2),
                    (ReflectionStyle.Json,false) => "".AsSpan(),
                    (ReflectionStyle.DebugView,false) => " ".AsSpan(),
                    (_,_) => "".AsSpan()
                };
            var delimiter = (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
                {
                    (_,true) => CommaCrLfSpaces400.AsSpan().Slice(0,5 + options.Depth*2),
                    (ReflectionStyle.Json, false) =>",",
                    (ReflectionStyle.DebugView, false) => ", ",
                    (_,_) => ", "
                };
            var separator = (options.ReflectionOptions.Style, options.JsonOptions.WriteIndented) switch
            {
                (ReflectionStyle.Json,true) => ": ",
                (ReflectionStyle.Json, false) =>":",
                (ReflectionStyle.DebugView, _) => " = ",
                (_,_) => " = "
            };

            if (value is null) return Null;

            if (IsScalarish(value.GetType()))
            {
                return ScalarishToShortReflectedString(value, options);
            }
            else if (value is ITuple valueTuple)
            {
                return DelimitTuple(valueTuple, options);
            }
            else if (value is IEnumerable enumerable)
            {
                return DelimitEnumerable(enumerable, options);
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
                        p => $"{qname(p.Name)}{separator}{TryGetValue(p)}"
                        ).ToList();

                var gotLock = sbLock.Wait(0);
                var sb_ = gotLock ? sb.Clear() : new StringBuilder();
                try
                {
                    sb.Append('{').Append(indent);
                    bool atleastone = false;
                    foreach (var line in contents)
                    {
                        sb.Append(line).Append(delimiter);
                        atleastone = true;
                    }
                    if(atleastone) sb.Length -= delimiter.Length;
                    sb.Append(outdent).Append('}');

                    return sb.ToString();
                }
                finally
                {
                    if (gotLock) sbLock.Release();
                }
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
                    return ScalarishToShortReflectedString(p.GetValue(value), options);
                else if(p.GetIndexParameters().Any())
                    return qname(p.PropertyType.Name);
                else
                    return BuildReflectedString(
                        p.GetValue(value),
                        options with { Depth = options.Depth + 1 });
            }
            catch
            {
                return qname(p.PropertyType.Name);
            }
        }

        bool IsScalarish(Type type) =>
            type.IsPrimitive
            || type.Namespace=="System.Numerics"
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(Type)
            || type == typeof(DateTime)
            || type == typeof(DateOnly)
            || type == typeof(TimeOnly)
            || type == typeof(TimeSpan)
            || (
                type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                && IsScalarish(type.GenericTypeArguments[0]))
            ;
    }

    static string DelimitTuple<T>(T value, OptionsWithState options) where T : ITuple
    {
        var (start, delimiter,end) = options.ReflectionOptions.Style == ReflectionStyle.Json
            ? ("[", "," , "]")
            : ("(", ", ", ")");
        var b = new StringBuilder(start);
        for (int i = 1; i < value.Length; i++)
        {
            b.Append(BuildReflectedString(value[ i -1 ],
                                    options with { Depth = options.Depth + 1 }));
            b.Append(delimiter);
        }
        b.Append(BuildReflectedString(value[ value.Length - 1 ],
                                options with { Depth = options.Depth + 1 }));
        b.Append(end);
        return b.ToString();
    }
    static string DelimitEnumerable<T>(T value, OptionsWithState options) where T : IEnumerable
    {
        if (options.Depth >= options.ReflectionOptions.MaxDepth)
        {
            return ScalarishToShortReflectedString(value,options);
        }
        if (options.Depth + 1 == options.ReflectionOptions.MaxDepth
            &&
            options.ReflectionOptions.Style == ReflectionStyle.DebugView
            &&
            typeof(T).GetMethod("ToString",BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.Public) is null)
        {
            //If the next level of reflection is only going to return object.ToString()
            //then better to halt at this level, where we can show more metadata i.e. Length
            return ScalarishToShortReflectedString(value,options);
        }
        var maxLength = options.ReflectionOptions.MaxLength;
        if (maxLength < 0) maxLength = - maxLength - options.Depth;

        if(maxLength == 0) return ScalarishToShortReflectedString(value,options);

        int i = 0;
        var (start, delimiter,end) = options.ReflectionOptions.Style == ReflectionStyle.Json
            ? ("[",  "," ,  "]")
            : ("[ ", ", ", " ]");
        var b = new StringBuilder(start);
        foreach(var item in value)
        {
            if(i > 0){ b.Append(delimiter); }
            b.Append(BuildReflectedString(item, options with { Depth = options.Depth + 1 }));

            if (++i >= maxLength) break;
        }
        b.Append(end);

        if(i == 0) return ScalarishToShortReflectedString(value,options);

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
        => BuildReflectedString(
            value,
            new OptionsWithState(0,
                TooStringOptions.Default with
                {
                    JsonOptions = TooStringOptions.DefaultJsonOptions
                        .With(o=> o.WriteIndented=indentedJson),
                    ReflectionOptions = ReflectionOptions.ForDebugView with
                    {
                        Style = style,
                        WhichProperties = whichProperties,
                    },
                })
        );

    static string ToJsonEscapedString(string s)
        => s.Replace("\\","\\\\")
            .Replace("\"","\\u0022")
            //.Replace("+","\\u002B") no need to escape + ?
            .Replace("`","\\u0060")
            .Replace("\t","\\\t")
            .Replace("\n","\\\n")
            .Replace("\r","\\\r")
            .Replace("\"","\\\"")
            .Replace("<","\\u003C")
            .Replace(">","\\u003E");

    static string ScalarishToShortReflectedString(object? value, OptionsWithState options)
    {
        var style = options.ReflectionOptions.Style;
        var isJson = style == ReflectionStyle.Json;

        Func<string, string> qstr =
                isJson
                    ? s => string.Format("\"{0}\"", ToJsonEscapedString(s))
                    : s => s
            ;
        Func<string, string> qEnum =
            isJson
                ? s => string.Format("\"{0}\"", ToJsonEscapedString(s))
                : s => s;

        Func<ValueType, string> numericToArrayIfJson = s =>
        {
            if(s is null) return Null;
            return
                isJson
                    ? $"[{
                        s.ToString()!
                         .TrimStart('(','<')
                         .TrimEnd(')','>')
                         .Replace(";",",")
                         .Replace(", ",",")
                    }]"
                    : s.ToString()!;
        };

        try
        {
            if(value is null) return Null;
            if(value is string s) return qstr(s);
            if (value.GetType().IsEnum) return qEnum(value.ToString()!);
            if (true.Equals(value)) return isJson ? "true" :"True";
            if (false.Equals(value)) return isJson ? "false" :"False";
            if (value.GetType().IsPrimitive) return value.ToString()!;
            if (value is System.Numerics.BigInteger b) return b.ToString();
            if (value is System.Numerics.Quaternion q) return isJson
                    ? q.ToString().Replace(' ',',')
                    : q.ToString();
            if (value is ValueType val && IsMultiDimensionalNumeric(val.GetType()))
                return numericToArrayIfJson(val);
            if (value is DateTime dateTime)
                return qstr(dateTime.ToString(options.ReflectionOptions.DateTimeFormat));
            if (value is DateOnly date)
                return qstr(date.ToString(options.ReflectionOptions.DateOnlyFormat));
            if (value is TimeOnly time)
                return qstr(time.ToString(options.ReflectionOptions.TimeOnlyFormat));
            if (value is TimeSpan span)
                return qstr(span.ToString(options.ReflectionOptions.TimeSpanFormat));
            if(value is Type type) return qstr( type.FullName??$"{type.Namespace}.{type.Name}" );

            if (value.GetType().IsAssignableTo(typeof(IEnumerable)))
            {
                if (isJson ) return "[]";
                if (value is Array arr) return $"{arr.GetType().GetElementType()}[{arr.Length}]";
                if (value is IList list && list.GetType().GetGenericArguments().Any())
                {
                    var collectionType = list.GetType().GetTypeInfo().Name;
                    var backtick=collectionType.IndexOf('`');
                    if(backtick>0)collectionType=collectionType.Substring(0,backtick);
                    var itemTypes = string.Join(",", list.GetType().GetGenericArguments().Select(a=>a.Name));
                    return $"{{ Type = {collectionType}<{itemTypes}>, Count = {list.Count} }}";
                }
                //return qstr(value.ToString() ?? Null);
            }

            return qstr(value.ToString() ?? Null);
        }
        catch
        {
            if(value is null) return Null;
            return qstr(value.GetType().Name);
        }
    }

   static readonly string CrLfSpaces400 = "\r\n" + new string(' ',400);
   static readonly string CommaCrLfSpaces400 = "," + CrLfSpaces400;
   static readonly StringBuilder sb = new StringBuilder();
   static readonly SemaphoreSlim sbLock = new(1);
   static bool IsMultiDimensionalNumeric(Type type)=>
       type.Namespace == "System.Numerics"
        &&
        new[]{"Complex","Vector","Vector2","Vector3","Vector4","Matrix3x2","Matrix4x4","Plane"}.Contains(type.Name);
}