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
    /// The complete set of stringification options. There are no options for
    /// <see cref="TooStringHow.CallerArgument"/>.
    /// </summary>
    /// <param name="ReflectionOptions">
    /// The options for <see cref="TooStringHow.Reflection"/>.
    /// The <see cref="Default"/> value is <see cref="ReflectionOptions.Default"/>
    /// which is
    /// <code>
    /// new ReflectionOptions(
    ///     BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    ///     ReflectionStyle Style = ReflectionStyle.Json,
    ///     int MaxDepth = 3,
    ///     int MaxLength = 9,
    ///     string DateTimeFormat = "O",
    ///     string DateOnlyFormat = "O",
    ///     string TimeOnlyFormat = "HH:mm:ss",
    ///     string TimeSpanFormat = "c")
    /// </code>
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
    /// What methods — <see cref="TooStringHow"/> — to try to stringify, and in what order.
    /// The <see cref="Default"/> value is a single entry of <see cref="TooStringHow.BestEffort"/>
    /// </param>
    public TooStringOptions(ReflectionOptions ReflectionOptions,
                            JsonSerializerOptions JsonOptions,
                            params IEnumerable<TooStringHow> Fallbacks)
    {
        this.ReflectionOptions = ReflectionOptions;
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
    ///     ReflectionOptions:ReflectionOptions.Default,
    ///     JsonSerializerOptions:DefaultJsonOptions,
    ///     Fallbacks:[TooStringHow.BestEffort],
    /// </c>
    /// </summary>
    public static readonly TooStringOptions Default =
        new(ReflectionOptions.ForDebugView,
            DefaultJsonOptions,
            new List<TooStringHow>(){TooStringHow.BestEffort }.AsReadOnly());

    /// <summary>
    /// The options for <see cref="TooStringHow.Reflection"/>.
    /// The <see cref="Default"/> value is <see cref="TooString.ReflectionOptions.ForDebugView"/>
    /// which is
    /// <code>
    /// public record ReflectionOptions(
    ///     BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    ///     ReflectionStyle Style = ReflectionStyle.Json,
    ///     int MaxDepth = 3,
    ///     int MaxLength = 9,
    ///     string DateTimeFormat = "O",
    ///     string DateOnlyFormat = "O",
    ///     string TimeOnlyFormat = "HH:mm:ss",
    ///     string TimeSpanFormat = "c")
    /// </code>
    /// </summary>
    public ReflectionOptions ReflectionOptions { get; init; }

    /// <summary>
    /// The <see cref="Default"/> value is <see cref="DefaultJsonOptions"/>, which has
    /// <code>
    /// DefaultJsonOptions = new(JsonSerializerDefaults.General)
    ///     {
    ///         ReferenceHandler = ReferenceHandler.IgnoreCycles
    ///     };
    /// </code>
    /// </summary>
    public JsonSerializerOptions JsonOptions { get; init; }

    /// <summary>
    /// What methods — <see cref="TooStringHow"/> — to try to stringify, and in what order.
    /// The <see cref="Default"/> value is a single entry of <see cref="TooStringHow.BestEffort"/>
    /// </summary>
    public IEnumerable<TooStringHow> Fallbacks { get; init; }

    /// <param name="reflectionOptions"></param>
    /// <param name="jsonOptions"></param>
    /// <param name="fallbacks"></param>
    public void Deconstruct(out ReflectionOptions reflectionOptions,
                            out JsonSerializerOptions jsonOptions,
                            out IEnumerable<TooStringHow> fallbacks)
    {
        reflectionOptions = ReflectionOptions;
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
    /// Default options for <see cref="TooStringHow.Json"/> with {JsonOptions modified by nonDefaults}
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
            Fallbacks = Default.Fallbacks.Prepend(TooStringHow.Json),
            ReflectionOptions = ReflectionOptions.ForJson
        };
    }
    ///<summary>
    /// Returns a <see cref="TooStringOptions"/> instance which will try reflection serialization
    ///  as its first choice, using either <see cref="Default"/> options, or else the
    /// supplied
    /// </summary>
    /// <param name="reflectionOptions">
    /// If not null, then return options with <see cref="JsonOptions"/> reconfigured
    /// from the default by applying <paramref name="reflectionOptions"/>.
    /// </param>
    /// <returns>
    /// Default options as modified by <paramref name="reflectionOptions"/>
    /// </returns>
    public static TooStringOptions ForReflection(ReflectionOptions? reflectionOptions = null)
    {
        return reflectionOptions is null
            ? Default with
                      {
                          Fallbacks = Default.Fallbacks.Prepend(TooStringHow.Reflection)
                      }
            : Default with
                      {
                          ReflectionOptions = reflectionOptions,
                          Fallbacks = Default.Fallbacks.Prepend(TooStringHow.Reflection)
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
        => new(ReflectionOptions.ForJson,
               jsonSerializerOptions,
               TooStringHow.Json);

    /// <summary>
    /// Create <see cref="TooStringOptions"/> from <paramref name="reflectionOptions"/>
    /// </summary>
    /// <param name="reflectionOptions"></param>
    /// <returns></returns>
    public static implicit operator TooStringOptions(ReflectionOptions reflectionOptions)
        => new(reflectionOptions,
               DefaultJsonOptions,
               TooStringHow.Reflection);

}

internal record OptionsWithState(int Depth,
                                 ReflectionOptions ReflectionOptions,
                                 JsonSerializerOptions JsonOptions,
                                 IEnumerable<TooStringHow> Fallbacks)
                : TooStringOptions(ReflectionOptions, JsonOptions, Fallbacks)
{
    public OptionsWithState(int depth, TooStringOptions @from)
           : this(depth, from.ReflectionOptions,from.JsonOptions, from.Fallbacks) { }
}