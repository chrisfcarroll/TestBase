using System.Text.Json;
using System.Text.Json.Serialization;

namespace TooString;

/// <summary>
/// The complete set of serialization options. When calling
/// <see cref="ObjectTooString.TooString{T}(T,TooString.TooStringStyle,System.Nullable{TooString.SerializationStyle},string?)"/>
/// the given parameters are turned into a <see cref="TooStringOptions"/> class to pass
/// to <see cref="ObjectTooString.TooString{T}(T,TooStringOptions,string?)"/>
/// </summary>
/// <param name="PreferenceOrder">
/// The <see cref="Default"/> value is a single entry of <see cref="TooStringStyle.BestEffort"/>
/// </param>
/// <param name="JsonOptions">
/// The <see cref="Default"/> value is <see cref="DefaultJsonOptions"/>, which has
/// <see cref="JsonSerializerOptions.ReferenceHandler"/>=<see cref="ReferenceHandler.IgnoreCycles"/>
/// </param>
/// <param name="ReflectionOptions">
/// The <see cref="Default"/> value is <see cref="ReflectionSerializerOptions.Default"/>
/// which specifies public instance properties and fields to a recursion depth of 3 and
/// option as Json. 
/// </param>
/// <param name="JsonDefaults">
/// The <see cref="Default"/> value is <see cref="JsonSerializerDefaults.General"/>
/// </param>
public record TooStringOptions(IEnumerable<TooStringStyle> PreferenceOrder,
                               JsonSerializerOptions JsonOptions,
                               ReflectionSerializerOptions ReflectionOptions,
                               JsonSerializerDefaults JsonDefaults = 
                                                      JsonSerializerDefaults.General)
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
    ///     PreferenceOrder:[TooStringStyle.ArgumentExpression, TooStringStyle.Json],
    ///     JsonSerializerOptions:DefaultJsonOptions
    ///     ReflectionSerializerOptions:ReflectionSerializerOptions.Default,
    ///     JsonSerializerDefaults:JsonSerializerDefaults.Web,
    /// </c>
    /// </summary>
    public static readonly TooStringOptions Default =
        new(
            new List<TooStringStyle>(){TooStringStyle.BestEffort }.AsReadOnly(),
            DefaultJsonOptions,
            ReflectionSerializerOptions.Default,
            JsonSerializerDefaults.General
        );

    public static implicit operator JsonSerializerOptions(TooStringOptions o)
        => o.JsonOptions;
}

internal record
    OptionsWithState(
        int Depth,
        IEnumerable<TooStringStyle> PreferenceOrder,
        JsonSerializerOptions JsonOptions,
        ReflectionSerializerOptions ReflectionOptions,
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