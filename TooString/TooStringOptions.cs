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
    /// The <see cref="Default"/> value is <see cref="TooString.AdvancedOptions.Default"/>
    /// </param>
    /// <param name="JsonOptions">
    /// The <see cref="Default"/> value is <see cref="DefaultJsonSerializerOptions"/>, which has
    /// <code>
    /// DefaultJsonOptions = new(JsonSerializerDefaults.General)
    ///     {
    ///         ReferenceHandler = ReferenceHandler.IgnoreCycles
    ///     };
    /// </code>
    /// </param>
    /// <param name="StringifyAs">
    /// What styles — <see cref="TooStringStyle"/> — to try to stringify, and in what order.
    /// The <see cref="Default"/> value is <see cref="TooStringStyle.CSharp"/>
    /// </param>
    public TooStringOptions(AdvancedOptions AdvancedOptions,
                            JsonSerializerOptions JsonOptions,
                            TooStringStyle StringifyAs)
    {
        this.AdvancedOptions = AdvancedOptions;
        this.JsonOptions = JsonOptions;
        this.StringifyAs = StringifyAs;
    }

    /// <summary><c>
    /// new(JsonSerializerDefaults.General)
    /// {
    ///    ReferenceHandler = ReferenceHandler.IgnoreCycles
    /// }</c>
    /// </summary>
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions =
        new(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

    /// <summary><c>
    /// new(
    ///     AdvancedOptions:AdvancedOptions.ForDebugView,
    ///     JsonSerializerOptions:DefaultJsonOptions,
    ///     TooStringStyle.JsonSerializer,
    /// </c>
    /// </summary>
    public static readonly TooStringOptions Default =
        new(AdvancedOptions.Default,
            DefaultJsonSerializerOptions,
            TooStringStyle.CSharp);

    /// <summary>
    /// Advanced options for reflected output.
    /// The <see cref="Default"/> value is <see cref="TooString.AdvancedOptions.Default"/>
    /// </summary>
    public AdvancedOptions AdvancedOptions { get; init; }

    /// <summary>
    /// Gets or sets whether output should use indents and newlines.
    /// Defaults to false
    /// </summary>
    public bool WriteIndented
    {
        get => JsonOptions.WriteIndented;
        set
        {
            if (JsonOptions.WriteIndented != value)
            {
                jsonOptions = new JsonSerializerOptions(JsonOptions) { WriteIndented = value };
            }
        }
    }

    JsonSerializerOptions jsonOptions;

    /// <summary>
    /// Options for System.Text.Json serialization.
    /// </summary>
    /// <remarks>
    /// ⚠️Only used when <see cref="StringifyAs"/> is <see cref="TooStringStyle.JsonSerializer"/>
    /// </remarks>
    public JsonSerializerOptions JsonOptions
    {
        get => jsonOptions;
        init => jsonOptions = value;
    }

    /// <summary>
    /// The <see cref="TooStringStyle"/> to stringify to.
    /// </summary>
    public TooStringStyle StringifyAs { get; init; }

    /// <param name="advancedOptions"></param>
    /// <param name="jsonOptions"></param>
    /// <param name="stringifyAs"></param>
    public void Deconstruct(out AdvancedOptions advancedOptions,
                            out JsonSerializerOptions jsonOptions,
                            out TooStringStyle stringifyAs)
    {
        advancedOptions = AdvancedOptions;
        jsonOptions = JsonOptions;
        stringifyAs = StringifyAs;
    }


    /// <summary>
    ///  Returns a <see cref="TooStringOptions"/> instance which will try json serialization
    ///  as its first choice, using either <see cref="DefaultJsonSerializerOptions"/>, or
    ///  <see cref="JsonOptions"/> as configured by <paramref name="jsDefaults"/> and
    ///  <paramref name="reconfigure"/>.
    ///  </summary>
    ///  <param name="reconfigure">
    ///  If not null, then return options with <see cref="JsonOptions"/> reconfigured
    ///  from the default by applying <paramref name="reconfigure"/>.
    ///  </param>
    ///  <param name="jsDefaults">
    ///  <see cref="JsonSerializerDefaults"/>
    ///  </param>
    /// <param name="serializeOrStringify">
    /// Specify <see cref="TooStringStyle.JsonStringifier"/> to force using our
    /// stringify without trying <see cref="System.Text.Json.JsonSerializer"/>
    /// </param>
    /// <returns>
    ///  Default options for <see cref="TooStringStyle.JsonSerializer"/> with {JsonOptions modified by nonDefaults}
    ///  </returns>
    public static TooStringOptions ForJson(Action<JsonSerializerOptions>? reconfigure = null,
                                           JsonSerializerDefaults jsDefaults = JsonSerializerDefaults.General,
                                           TooStringStyle serializeOrStringify = TooStringStyle.JsonSerializer)
    {
        var js= new JsonSerializerOptions(jsDefaults)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        };
        reconfigure?.Invoke(js);
        if (serializeOrStringify is not TooStringStyle.JsonSerializer)
        {
            serializeOrStringify = TooStringStyle.JsonSerializer;
        }
        return Default with
        {
            JsonOptions = js,
            StringifyAs = serializeOrStringify,
            AdvancedOptions = AdvancedOptions.Default
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
        return advancedOptions is null
            ? Default
            : Default with
                      {
                          AdvancedOptions = advancedOptions,
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
        => new(AdvancedOptions.Default,
               jsonSerializerOptions,
               TooStringStyle.JsonSerializer);

    /// <summary>
    /// Create <see cref="TooStringOptions"/> from <paramref name="advancedOptions"/>
    /// </summary>
    /// <param name="advancedOptions"></param>
    /// <returns></returns>
    public static implicit operator TooStringOptions(AdvancedOptions advancedOptions)
        => new(advancedOptions,DefaultJsonSerializerOptions,TooStringStyle.JsonSerializer);

}

internal record OptionsWithState(int Depth,
                                 AdvancedOptions AdvancedOptions,
                                 JsonSerializerOptions JsonOptions,
                                 TooStringStyle StringifyAs)
                : TooStringOptions(AdvancedOptions, JsonOptions, StringifyAs)
{
    public OptionsWithState(int depth, TooStringOptions @from)
           : this(depth, from.AdvancedOptions,from.JsonOptions, from.StringifyAs) { }
}
