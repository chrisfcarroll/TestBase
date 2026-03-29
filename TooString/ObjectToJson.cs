using System.Runtime.CompilerServices;
using System.Text.Json;

namespace TooString;

public static partial class ObjectTooString
{

    /// <summary>
    /// Try to use <see cref="JsonSerializer"/> to serialize <paramref name="value"/>.
    /// If that fails — for instance, for types in System.Reflection, and for System.Type itself,
    /// returns <see cref="TooString{T}(T,TooStringOptions)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// System.Text.Json.JsonSerializer.Serialize(value,tooStringOptions.JsonOptions), unless that fails,
    /// in which case <see cref="TooString{T}(T,TooStringOptions)"/> is called.
    /// </returns>
    public static string ToJson<T>(this T? value, TooStringOptions options)
    {
        if (typeof(T).FullName == "System.Type"
            ||
            typeof(T).FullName?.StartsWith("System.Reflection") is true
            ||
            (value is ITuple && !options.JsonOptions.IncludeFields)
           )
        {
            options = options with {StringifyAs = TooStringStyle.JsonStringifier};
            return BuildReflectedString(value, new OptionsWithState(0, options));
        }
        else try
            {
                return JsonSerializer.Serialize(value,options.JsonOptions);
            }
            catch
            {
                options = options with {StringifyAs = TooStringStyle.JsonStringifier};
                return BuildReflectedString(value, new OptionsWithState(0, options));;
            }
    }

    /// <summary>
    /// Try to serialize <paramref name="value"/> using System.Text.Json. Failing that
    /// </summary>
    /// <param name="value"></param>
    /// <param name="writeIndented"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToJson<T>(this T? value, bool writeIndented = false)
        => ToJson(value,
                  TooStringOptions.ForJson(o=>o.WriteIndented=writeIndented));


    /// <summary>
    /// Serialize <paramref name="value"/> using System.Text.Json. Throws if serialization fails.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToJson<T>(this T? value, JsonSerializerOptions jsonSerializerOptions)
        => JsonSerializer.Serialize(value,jsonSerializerOptions);

}