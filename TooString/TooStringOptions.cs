using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
[assembly:InternalsVisibleTo("TooString.Specs")]

namespace TooString;

/// <summary>
/// The complete set of serialization options. When calling
/// <see cref="ObjectTooString.TooString{T}(T,TooStringHow,System.Nullable{ReflectionStyle},string?)"/>
/// the given parameters are turned into a <see cref="TooStringOptions"/> class to pass
/// to <see cref="ObjectTooString.TooString{T}(T,TooStringOptions,string?)"/>
/// </summary>
/// <param name="PreferenceOrder">
/// The <see cref="Default"/> value is a single entry of <see cref="TooStringHow.BestEffort"/>
/// </param>
/// <param name="JsonOptions">
/// The <see cref="Default"/> value is <see cref="DefaultJsonOptions"/>, which has
/// <see cref="JsonSerializerOptions.ReferenceHandler"/>=<see cref="ReferenceHandler.IgnoreCycles"/>
/// </param>
/// <param name="ReflectionOptions">
/// The <see cref="Default"/> value is <see cref="ReflectionOptions.Default"/>
/// which specifies public instance properties and fields to a recursion depth of 3 and
/// option as Json. 
/// </param>
/// <param name="JsonDefaults">
/// The <see cref="Default"/> value is <see cref="JsonSerializerDefaults.General"/>
/// </param>
public record TooStringOptions(IEnumerable<TooStringHow> PreferenceOrder,
                               JsonSerializerOptions JsonOptions,
                               ReflectionOptions ReflectionOptions,
                               JsonSerializerDefaults JsonDefaults = JsonSerializerDefaults.General)
{
    /// <summary><c>
    /// new(JsonSerializerDefaults.Web)
    /// {
    ///    ReferenceHandler = ReferenceHandler.IgnoreCycles
    /// }</c>
    /// </summary>
    internal static readonly JsonSerializerOptions DefaultJsonOptions =
        new(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

    /// <summary><c>
    /// new(
    ///     PreferenceOrder:[TooStringHow.ArgumentExpression, TooStringHow.Json],
    ///     JsonSerializerOptions:DefaultJsonOptions
    ///     ReflectionOptions:ReflectionOptions.Default,
    ///     JsonSerializerDefaults:JsonSerializerDefaults.Web,
    /// </c>
    /// </summary>
    public static readonly TooStringOptions Default =
        new(
            new List<TooStringHow>(){TooStringHow.BestEffort }.AsReadOnly(),
            DefaultJsonOptions,
            ReflectionOptions.Default,
            JsonSerializerDefaults.General
        );

    /// <summary>
    /// Extract the <see cref="JsonOptions"/> out of <paramref name="o"/>
    /// </summary>
    /// <param name="o"></param>
    /// <returns><c>o.JsonOptions</c></returns>
    public static implicit operator JsonSerializerOptions(TooStringOptions o)
        => o.JsonOptions;

#if NET8_0_OR_GREATER
    static TooStringOptions()
    {
        DefaultJsonOptions.MakeReadOnly();
    }
#endif

}

internal record
    OptionsWithState(
        int Depth,
        IEnumerable<TooStringHow> PreferenceOrder,
        JsonSerializerOptions JsonOptions,
        ReflectionOptions ReflectionOptions,
        JsonSerializerDefaults JsonDefaults = JsonSerializerDefaults.General
    )
    : TooStringOptions(PreferenceOrder, JsonOptions, ReflectionOptions, JsonDefaults)
{
    public OptionsWithState(int depth, TooStringOptions @from) : this(depth,
        @from.PreferenceOrder, @from.JsonOptions, @from.ReflectionOptions,
        @from.JsonDefaults)
    {
    }
}