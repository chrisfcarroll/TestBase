using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
[assembly:InternalsVisibleTo("TooString.Specs")]

namespace TooString;

/// <summary>
/// The complete set of stringification options.
/// </summary>
public record TooStringOptions
{
    /// <summary>
    /// The complete set of stringification options.
    /// </summary>
    /// <param name="AdvancedOptions">
    /// The options for reflection-based styles
    /// (<see cref="TooStringStyle.DebugView"/>, <see cref="TooStringStyle.JsonStringifier"/>,
    /// <see cref="TooStringStyle.CSharp"/>).
    /// The <see cref="Default"/> value is <see cref="AdvancedOptions.ForCSharp"/>
    /// </param>
    /// <param name="JsonOptions">
    /// The <see cref="Default"/> value is <see cref="DefaultJsonOptions"/>, which has
    /// <code>
    /// DefaultJsonOptions = new(JsonSerializerDefaults.General)
    ///     {
    ///         ReferenceHandler = ReferenceHandler.IgnoreCycles
    ///     };
    /// </code>
    /// </param>
    /// <param name="Fallbacks">
    /// What styles — <see cref="TooStringStyle"/> — to try to stringify, and in what order.
    /// The <see cref="Default"/> value is a single entry of <see cref="TooStringStyle.BestEffort"/>
    /// </param>
    public TooStringOptions(AdvancedOptions AdvancedOptions,
                            JsonSerializerOptions JsonOptions,
                            params IEnumerable<TooStringStyle> Fallbacks)
    {
        this.AdvancedOptions = AdvancedOptions;
        this.JsonOptions = JsonOptions;
        this.Fallbacks = Fallbacks;
    }

    /// <summary><c>
    /// new(JsonSerializerDefaults.General)
    /// {
    ///    ReferenceHandler = ReferenceHandler.IgnoreCycles
    /// }</c>
    /// </summary>
    public static readonly JsonSerializerOptions DefaultJsonOptions =
        new(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

    /// <summary><c>
    /// new(
    ///     AdvancedOptions:AdvancedOptions.ForDebugView,
    ///     JsonSerializerOptions:DefaultJsonOptions,
    ///     Fallbacks:[TooStringStyle.BestEffort],
    /// </c>
    /// </summary>
    public static readonly TooStringOptions Default =
        new(AdvancedOptions.ForCSharp,
            DefaultJsonOptions,
            new List<TooStringStyle>(){TooStringStyle.BestEffort }.AsReadOnly());

    /// <summary>
    /// The options for reflection-based styles.
    /// The <see cref="Default"/> value is <see cref="AdvancedOptions.ForCSharp"/>
    /// </summary>
    public AdvancedOptions AdvancedOptions { get; init; }

    /// <summary>
    /// The <see cref="Default"/> value is <see cref="DefaultJsonOptions"/>.
    /// </summary>
    public JsonSerializerOptions JsonOptions { get; init; }

    /// <summary>
    /// What styles — <see cref="TooStringStyle"/> — to try to stringify, and in what order.
    /// The <see cref="Default"/> value is a single entry of <see cref="TooStringStyle.BestEffort"/>
    /// </summary>
    public IEnumerable<TooStringStyle> Fallbacks { get; init; }

    /// <param name="advancedOptions"></param>
    /// <param name="jsonOptions"></param>
    /// <param name="fallbacks"></param>
    public void Deconstruct(out AdvancedOptions advancedOptions,
                            out JsonSerializerOptions jsonOptions,
                            out IEnumerable<TooStringStyle> fallbacks)
    {
        advancedOptions = AdvancedOptions;
        jsonOptions = JsonOptions;
        fallbacks = Fallbacks;
    }

    ///<summary>
    /// Returns a <see cref="TooStringOptions"/> instance which will try json serialization
    /// as its first choice, using either <see cref="DefaultJsonOptions"/>, or
    /// <see cref="JsonOptions"/> as configured by <paramref name="jsDefaults"/> and
    /// <paramref name="reconfigure"/>.
    /// </summary>
    /// <param name="reconfigure">
    /// If not null, then return options with <see cref="JsonOptions"/> reconfigured
    /// from the default by applying <paramref name="reconfigure"/>.
    /// </param>
    /// <param name="jsDefaults">
    /// <see cref="JsonSerializerDefaults"/>
    /// </param>
    /// <returns>
    /// Default options for <see cref="TooStringStyle.JsonSerializer"/> with {JsonOptions modified by nonDefaults}
    /// </returns>
    public static TooStringOptions ForJson(Action<JsonSerializerOptions>? reconfigure = null,
                                           JsonSerializerDefaults jsDefaults = JsonSerializerDefaults.General)
    {
        var js= new JsonSerializerOptions(jsDefaults)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        };
        reconfigure?.Invoke(js);
        return Default with
        {
            JsonOptions = js,
            Fallbacks = Default.Fallbacks.Prepend(TooStringStyle.JsonSerializer),
            AdvancedOptions = AdvancedOptions.ForJson
        };
    }

    ///<summary>
    /// Returns a <see cref="TooStringOptions"/> instance which will try reflection serialization
    /// as its first choice, using either <see cref="Default"/> options, or else the
    /// supplied <paramref name="advancedOptions"/>.
    /// </summary>
    /// <param name="advancedOptions">
    /// If not null, then return options with <see cref="AdvancedOptions"/> set to
    /// <paramref name="advancedOptions"/>.
    /// </param>
    /// <returns>
    /// Default options as modified by <paramref name="advancedOptions"/>
    /// </returns>
    public static TooStringOptions WithAdvancedOptions(AdvancedOptions? advancedOptions = null)
    {
        var style = advancedOptions?.Style switch
        {
            ReflectionStyle.Json => TooStringStyle.JsonStringifier,
            ReflectionStyle.CSharp => TooStringStyle.CSharp,
            _ => TooStringStyle.DebugView
        };
        return advancedOptions is null
            ? Default with
                      {
                          Fallbacks = Default.Fallbacks.Prepend(TooStringStyle.DebugView)
                      }
            : Default with
                      {
                          AdvancedOptions = advancedOptions,
                          Fallbacks = Default.Fallbacks.Prepend(style)
                      };
    }

    /// <summary>
    /// Re-configured the current options by applying
    /// configuration action <paramref name="reconfigure"/>,
    /// and return the re-configured options.
    /// </summary>
    /// <param name="reconfigure"></param>
    /// <returns>
    /// a copy of the current options reconfigured with <paramref name="reconfigure"/>
    /// </returns>
    public TooStringOptions With(Action<TooStringOptions> reconfigure)
    {
        var mutated = this with { };
        reconfigure(mutated);
        return mutated;
    }

    /// <summary>
    /// Extract the <see cref="JsonOptions"/> out of <paramref name="o"/>
    /// </summary>
    /// <param name="o"></param>
    /// <returns><c>o.JsonOptions</c></returns>
    public static implicit operator JsonSerializerOptions(TooStringOptions o)
        => o.JsonOptions;

    /// <summary>
    /// Create <see cref="TooStringOptions"/> from <paramref name="jsonSerializerOptions"/>
    /// </summary>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public static implicit operator TooStringOptions(JsonSerializerOptions jsonSerializerOptions)
        => new(AdvancedOptions.ForJson,
               jsonSerializerOptions,
               TooStringStyle.JsonSerializer);

    /// <summary>
    /// Create <see cref="TooStringOptions"/> from <paramref name="advancedOptions"/>
    /// </summary>
    /// <param name="advancedOptions"></param>
    /// <returns></returns>
    public static implicit operator TooStringOptions(AdvancedOptions advancedOptions)
        => new(advancedOptions,
               DefaultJsonOptions,
               advancedOptions.Style switch
               {
                   ReflectionStyle.Json => TooStringStyle.JsonStringifier,
                   ReflectionStyle.CSharp => TooStringStyle.CSharp,
                   _ => TooStringStyle.DebugView
               });

}

internal record OptionsWithState(int Depth,
                                 AdvancedOptions AdvancedOptions,
                                 JsonSerializerOptions JsonOptions,
                                 IEnumerable<TooStringStyle> Fallbacks)
                : TooStringOptions(AdvancedOptions, JsonOptions, Fallbacks)
{
    public OptionsWithState(int depth, TooStringOptions @from)
           : this(depth, from.AdvancedOptions,from.JsonOptions, from.Fallbacks) { }
}
