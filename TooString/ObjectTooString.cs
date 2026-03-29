using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace TooString;

/// <summary>
/// A class for creating human-readable strings for objects.
/// It differs from a serializer primarily in <b>not</b> being fail-fast.
/// If it can't stringify an object correctly, it will return a best-effort
/// rather than fail.
/// For the stringify styles offered, <see cref="TooStringStyle"/>
/// </summary>
public static partial class ObjectTooString
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


    /// <param name="value"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// Returns the <see cref="CallerArgumentExpressionAttribute"/> string
    /// for <paramref name="value"/>.
    /// </returns>
    public static string ToArgumentExpression<T>(this T? value,
                                                 [CallerArgumentExpression("value")]
                                                 string expression="") => expression;

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
    static string ToStringified<T>(this T? value, TooStringOptions options)
        => BuildReflectedString(value, new OptionsWithState(0, options));

    /// <summary>
    /// Stringifies a value using the specified style.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="style">The <em>style</em> by which the value is
    /// stringified.
    /// </param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> according to the
    /// <paramref name="style"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value,
                                      TooStringStyle style = TooStringStyle.JsonSerializer,
                                      TooStringOptions? options = null)
    {
        options = (options??TooStringOptions.Default) with { StringifyAs = style };
        return TooString(value,options);
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
    /// Choose between <see cref="TooStringStyle.JsonStringifier"/>,
    /// <see cref="TooStringStyle.DebugView"/>, or
    /// <see cref="TooStringStyle.CSharp"/> (valid C# syntax with type names in comments)
    /// </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> according to the
    /// <paramref name="style"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value,
                                      int maxDepth,
                                      int maxLength = 9,
                                      TooStringStyle style = TooStringStyle.JsonStringifier)
    {
        var options = TooStringOptions.Default with
        {
            StringifyAs = style,
            AdvancedOptions = TooStringOptions.Default.AdvancedOptions
                with
                {
                    Style = style,
                    MaxDepth = maxDepth,
                    MaxEnumerationLength = maxLength
                }
        };
        return TooString(value,options);
    }

    /// <summary>
    /// Stringifies a value using reflection, with the options specified.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="advancedOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> built by recursively
    /// reflecting the properties of <paramref name="value"/> using the
    /// <paramref name="advancedOptions"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value, AdvancedOptions advancedOptions)
    {
        var options = TooStringOptions.Default with
        {
            StringifyAs = advancedOptions.Style,
            AdvancedOptions = advancedOptions
        };
        return TooString(value,options);
    }

    /// <summary>
    /// Stringifies a value using the configured options.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="tooStringOptions">Configure the style used, and the options
    /// relevant to the style used.
    /// See <see cref="TooStringOptions"/> for details of what can be configured.
    /// The easy way to change options may be to use <see cref="TooStringOptions.Default"/>
    /// with a <c>with</c> expression: <c>TooStringOptions.Default with { ... }</c>
    /// </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> according to the
    /// <paramref name="tooStringOptions"/> chosen.
    /// </returns>
    public static string TooString<T>(this T value, TooStringOptions tooStringOptions)
    {
        return tooStringOptions.StringifyAs is TooStringStyle.JsonSerializer
            ? ToJson(value,tooStringOptions)
            : BuildReflectedString(value,new OptionsWithState(0,tooStringOptions));
    }

    static string BuildReflectedString<T>(T? value, OptionsWithState options)
    {
        if (options.Depth >= options.AdvancedOptions.MaxDepth)
        {
            return ScalarishToShortReflectedString(value,options);
        }

        Func<string,string> qname =
            options.StringifyAs is TooStringStyle.JsonStringifier or TooStringStyle.JsonSerializer
                ? s => $"\"{s?.Replace("`","\\u0060").Replace("\"","\\\"")}\""
                : s => s;

        var isCSharp = options.StringifyAs == TooStringStyle.CSharp;
        try
        {
            var indent=
                (options.StringifyAs, options.JsonOptions.WriteIndented) switch
                {
                    (_,true) => NewLineSpaces400.AsSpan().Slice(0, 1 + (options.Depth + 1) * 2),
                    (TooStringStyle.JsonStringifier,false) => "".AsSpan(),
                    (TooStringStyle.JsonSerializer,false) => "".AsSpan(),
                    (TooStringStyle.DebugView,false) => " ".AsSpan(),
                    (TooStringStyle.CSharp,false) => " ".AsSpan(),
                    (_,_) => " ".AsSpan()
                };
            var outdent =
                (options.StringifyAs, options.JsonOptions.WriteIndented) switch
                {
                    (TooStringStyle.JsonStringifier,true) => NewLineSpaces400.AsSpan().Slice(0, 1 + (options.Depth) * 2),
                    (TooStringStyle.JsonStringifier,false) => "".AsSpan(),
                    (TooStringStyle.JsonSerializer,true) => NewLineSpaces400.AsSpan().Slice(0, 1 + (options.Depth) * 2),
                    (TooStringStyle.JsonSerializer,false) => "".AsSpan(),
                    (TooStringStyle.CSharp,true) => NewLineSpaces400.AsSpan().Slice(0, options.Depth * 2),
                    (TooStringStyle.CSharp,false) => " ".AsSpan(),
                    (TooStringStyle.DebugView,true) => NewLineSpaces400.AsSpan().Slice(0, options.Depth * 2),
                    (TooStringStyle.DebugView,false) => " ".AsSpan(),
                    (_,_) => "".AsSpan()
                };
            var delimiter = (options.StringifyAs, options.JsonOptions.WriteIndented) switch
                {
                    (TooStringStyle.JsonStringifier,true) => CommaCrLfSpaces400.AsSpan().Slice(0,2 + (options.Depth + 1) *2),
                    (TooStringStyle.JsonStringifier, false) =>",",
                    (TooStringStyle.JsonSerializer,true) => CommaCrLfSpaces400.AsSpan().Slice(0,2 + (options.Depth + 1) *2),
                    (TooStringStyle.JsonSerializer, false) =>",",
                    (TooStringStyle.CSharp,true) => CommaCrLfSpaces400.AsSpan().Slice(0,5 + options.Depth*2),
                    (TooStringStyle.CSharp, false) => ", ",
                    (TooStringStyle.DebugView,true) => CommaCrLfSpaces400.AsSpan().Slice(0,5 + options.Depth*2),
                    (TooStringStyle.DebugView, false) => ", ",
                    (_,_) => ", "
                };
            var separator = (options.StringifyAs, options.JsonOptions.WriteIndented) switch
            {
                (TooStringStyle.JsonStringifier,true) => ": ",
                (TooStringStyle.JsonStringifier, false) =>":",
                (TooStringStyle.JsonSerializer,true) => ": ",
                (TooStringStyle.JsonSerializer, false) =>":",
                (TooStringStyle.DebugView, _) => " = ",
                (TooStringStyle.CSharp, _) => " = ",
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
                    .GetProperties(options.AdvancedOptions.WhichProperties)
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
                    if (isCSharp && !value.GetType().Name.StartsWith("<>"))
                    {
                        var typeName = value.GetType().Name;
                        var backtick = typeName.IndexOf('`');
                        if (backtick > 0) typeName = typeName.Substring(0, backtick);
                        sb.Append("/*").Append(typeName).Append("*/ new {").Append(indent);
                    }
                    else
                    {
                        sb.Append('{').Append(indent);
                    }
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
                    || options.Depth > options.AdvancedOptions.MaxDepth)
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
        var (start, delimiter,end) = options.StringifyAs is TooStringStyle.JsonStringifier or TooStringStyle.JsonSerializer
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
        if (options.Depth >= options.AdvancedOptions.MaxDepth)
        {
            return ScalarishToShortReflectedString(value,options);
        }
        if (options.Depth + 1 == options.AdvancedOptions.MaxDepth
            &&
            options.StringifyAs is TooStringStyle.DebugView or TooStringStyle.CSharp
            &&
            typeof(T).GetMethod("ToString",BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.Public) is null)
        {
            //If the next level of reflection is only going to return object.ToString()
            //then better to halt at this level, where we can show more metadata i.e. Length
            return ScalarishToShortReflectedString(value,options);
        }
        var maxLength = options.AdvancedOptions.MaxEnumerationLength;
        if (maxLength < 0) maxLength = - maxLength - options.Depth;

        if(maxLength == 0) return ScalarishToShortReflectedString(value,options);

        int i = 0;
        var (start, delimiter, end) = options.StringifyAs switch
        {
            TooStringStyle.JsonStringifier or TooStringStyle.JsonSerializer=> ("[", ",", "]"),
            TooStringStyle.CSharp => ("new[] { ", ", ", " }"),
            _ => ("[ ", ", ", " ]")
        };
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
    /// Stringify <paramref name="value"/> using reflection.
    /// <list type="bullet">
    /// <item><see cref="TooStringStyle.JsonStringifier"/>: <c>{"A":1}</c></item>
    /// <item><see cref="TooStringStyle.CSharp"/>: <c>/*TypeName*/ new { A = 1 }</c> (valid C# syntax)</item>
    /// <item><see cref="TooStringStyle.DebugView"/>: <c>TypeName { A = 1 }</c></item>
    /// </list>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="style"></param>
    /// <param name="whichProperties"></param>
    /// <param name="indentedJson">Only relevant if <paramref name="style"/>
    ///     is <see cref="TooStringStyle.JsonStringifier"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToStringified<T>(this T? value,
                                              TooStringStyle style = TooStringStyle.CSharp,
                                              BindingFlags whichProperties =
                                                  BindingFlags.Instance | BindingFlags.Public,
                                              bool indentedJson = false)
        => BuildReflectedString(
            value,
            new OptionsWithState(0,
                TooStringOptions.Default with
                {
                    StringifyAs = style,
                    JsonOptions = TooStringOptions.DefaultJsonOptions
                        .With(o=> o.WriteIndented=indentedJson),
                    AdvancedOptions = AdvancedOptions.ForCSharp with
                    {
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
        var style = options.StringifyAs;
        var isJson = style is TooStringStyle.JsonStringifier or TooStringStyle.JsonSerializer;
        var isCSharp = style == TooStringStyle.CSharp;

        Func<string, string> qstr =
                isJson || isCSharp
                    ? s => string.Format("\"{0}\"", ToJsonEscapedString(s))
                    : s => s
            ;
        Func<string, string> qEnum =
            isJson
                ? s => string.Format("\"{0}\"", ToJsonEscapedString(s))
                : isCSharp && value != null
                    ? s => $"{value.GetType().Name}.{s}"
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
            if (true.Equals(value)) return isJson || isCSharp ? "true" : "True";
            if (false.Equals(value)) return isJson || isCSharp ? "false" : "False";
            if (value.GetType().IsPrimitive) return value.ToString()!;
            if (value is System.Numerics.BigInteger b) return b.ToString();
            if (value is System.Numerics.Quaternion q) return isJson
                    ? q.ToString().Replace(' ',',')
                    : q.ToString();
            if (value is ValueType val && IsMultiDimensionalNumeric(val.GetType()))
                return numericToArrayIfJson(val);
            if (value is DateTime dateTime)
                return qstr(dateTime.ToString(options.AdvancedOptions.DateTimeFormat));
            if (value is DateOnly date)
                return qstr(date.ToString(options.AdvancedOptions.DateOnlyFormat));
            if (value is TimeOnly time)
                return qstr(time.ToString(options.AdvancedOptions.TimeOnlyFormat));
            if (value is TimeSpan span)
                return qstr(span.ToString(options.AdvancedOptions.TimeSpanFormat));
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

   static readonly string NewLineSpaces400 = Environment.NewLine + new string(' ',400);
   static readonly string CommaCrLfSpaces400 = "," + NewLineSpaces400;
   static readonly StringBuilder sb = new StringBuilder();
   static readonly SemaphoreSlim sbLock = new(1);
   static bool IsMultiDimensionalNumeric(Type type)=>
       type.Namespace == "System.Numerics"
        &&
        new[]{"Complex","Vector","Vector2","Vector3","Vector4","Matrix3x2","Matrix4x4","Plane"}.Contains(type.Name);
}
