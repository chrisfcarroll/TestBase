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
    ///     PreferenceOrder:[TooStringHow.ArgumentExpression, TooStringHow.Json],
    ///     JsonSerializerOptions:DefaultJsonOptions
    ///     ReflectionOptions:ReflectionOptions.Default,
    ///     JsonSerializerDefaults:JsonSerializerDefaults.Web,
    /// </c>
    /// </summary>
    public static readonly TooStringOptions Default =
        new(ReflectionOptions.Default,
            DefaultJsonOptions,
            new List<TooStringHow>(){TooStringHow.BestEffort }.AsReadOnly());

    /// <summary>
    /// Extract the <see cref="JsonOptions"/> out of <paramref name="o"/>
    /// </summary>
    /// <param name="o"></param>
    /// <returns><c>o.JsonOptions</c></returns>
    public static implicit operator JsonSerializerOptions(TooStringOptions o)
        => o.JsonOptions;

    /// <summary>
    /// The options for <see cref="TooStringHow.Reflection"/>.
    /// The <see cref="Default"/> value is <see cref="ReflectionOptions.Default"/>
    /// which is
    /// <code>
    /// new ReflectionOptions(
    ///     BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    ///     ReflectionStyle Style = ReflectionStyle.Json,
    ///     int MaxDepth = 3,
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

    /// <param name="nonDefaults"></param>
    /// <returns>
    /// Default options for <see cref="TooStringHow.Json"/> with {JsonOptions modified by nonDefaults}
    /// </returns>
    public static TooStringOptions ForJson(Action<JsonSerializerOptions>? nonDefaults = null)
    {
        var js= new JsonSerializerOptions();
        nonDefaults?.Invoke(js);
        return Default with
        {
            JsonOptions = js,
            Fallbacks = Default.Fallbacks.Prepend(TooStringHow.Json),
            ReflectionOptions = ReflectionOptions.Default with {Style = ReflectionStyle.Json}
        };
    }
    /// <param name="nonDefaults"></param>
    /// <returns>
    /// Default options for <see cref="TooStringHow.Reflection"/>
    /// with { ReflectionOptions = nonDefaults}
    /// </returns>
    public static TooStringOptions ForReflection(ReflectionOptions nonDefaults)
    {
        return Default with
        {
            ReflectionOptions = nonDefaults,
            Fallbacks = Default.Fallbacks.Prepend(TooStringHow.Reflection)
        };
    }
}

internal record OptionsWithState(int Depth,
                                 IEnumerable<TooStringHow> Fallbacks,
                                 JsonSerializerOptions JsonOptions,
                                 ReflectionOptions ReflectionOptions)
                : TooStringOptions(ReflectionOptions, JsonOptions,Fallbacks)
{
    public OptionsWithState(int depth, TooStringOptions @from)
           : this(depth,@from.Fallbacks, @from.JsonOptions, @from.ReflectionOptions) { }
}