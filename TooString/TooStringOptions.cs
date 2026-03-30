using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
[assembly:InternalsVisibleTo("TooString.Specs")]

namespace TooString;

/// <summary>
/// The complete set of stringification options.
/// Use <c>new TooStringOptions { StringifyAs = ..., WriteIndented = true }</c>
/// or start from a preset like <see cref="ForJson"/> or <see cref="ForCSharp"/>
/// and customise with <see cref="With(TooStringOptions)"/> or
/// <see cref="With(bool?,TooStringStyle?,BindingFlags?,int?,int?,string?,string?,string?,string?)"/>.
/// </summary>
public record TooStringOptions
{
    /// <summary>
    /// Parameterless constructor with sensible defaults.
    /// All properties are settable via init or set, so you can use
    /// <c>new TooStringOptions { StringifyAs = TooStringStyle.JsonSerializer, WriteIndented = true }</c>
    /// </summary>
    public TooStringOptions()
    {
        AdvancedOptions = AdvancedOptions.Default;
        jsonOptions = DefaultJsonSerializerOptions;
        StringifyAs = TooStringStyle.CSharp;
    }

    /// <summary>
    /// Constructor taking all top-level options.
    /// </summary>
    public TooStringOptions(AdvancedOptions AdvancedOptions,
                            JsonSerializerOptions JsonOptions,
                            TooStringStyle StringifyAs)
    {
        this.AdvancedOptions = AdvancedOptions;
        this.jsonOptions = JsonOptions;
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

    /// <summary>
    /// Default options: CSharp style, default AdvancedOptions, default JsonSerializerOptions.
    /// </summary>
    public static readonly TooStringOptions Default =
        new(AdvancedOptions.Default,
            DefaultJsonSerializerOptions,
            TooStringStyle.CSharp);

    /// <summary>
    /// Preset for JSON serialization via <see cref="System.Text.Json.JsonSerializer"/>.
    /// Customise further with <c>.With(...)</c> or <c>with { ... }</c>.
    /// </summary>
    public static TooStringOptions ForJson
        => Default with { StringifyAs = TooStringStyle.JsonSerializer };

    /// <summary>
    /// Preset for CSharp-style output via reflection.
    /// Customise further with <c>.With(...)</c> or <c>with { ... }</c>.
    /// </summary>
    public static TooStringOptions ForCSharp
        => Default with { StringifyAs = TooStringStyle.CSharp };

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

    // ──────────────────────────────────────────────
    //  With() overloads
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns a copy of the current options reconfigured by applying the
    /// <paramref name="reconfigure"/> action.
    /// </summary>
    public TooStringOptions With(Action<TooStringOptions> reconfigure)
    {
        var mutated = this with { };
        reconfigure(mutated);
        return mutated;
    }

    /// <summary>
    /// Returns a copy of the current options with all values replaced by
    /// those from <paramref name="other"/>.
    /// Useful for programmatic composition of option sets.
    /// </summary>
    public TooStringOptions With(TooStringOptions other)
        => this with
        {
            AdvancedOptions = other.AdvancedOptions,
            JsonOptions = other.JsonOptions,
            StringifyAs = other.StringifyAs,
        };

    /// <summary>
    /// Returns a copy of the current options with
    /// <see cref="AdvancedOptions"/> replaced by <paramref name="advancedOptions"/>.
    /// </summary>
    public TooStringOptions With(AdvancedOptions advancedOptions)
        => this with { AdvancedOptions = advancedOptions };

    /// <summary>
    /// Returns a copy of the current options with any supplied parameters overridden.
    /// Parameters left as <c>null</c> retain their current value.
    /// </summary>
    public TooStringOptions With(
        bool? writeIndented = null,
        TooStringStyle? stringifyAs = null,
        BindingFlags? whichProperties = null,
        int? maxDepth = null,
        int? maxEnumerationLength = null,
        string? dateTimeFormat = null,
        string? dateOnlyFormat = null,
        string? timeOnlyFormat = null,
        string? timeSpanFormat = null)
    {
        var advanced = AdvancedOptions with
        {
            WhichProperties = whichProperties ?? AdvancedOptions.WhichProperties,
            MaxDepth = maxDepth ?? AdvancedOptions.MaxDepth,
            MaxEnumerationLength = maxEnumerationLength ?? AdvancedOptions.MaxEnumerationLength,
            DateTimeFormat = dateTimeFormat ?? AdvancedOptions.DateTimeFormat,
            DateOnlyFormat = dateOnlyFormat ?? AdvancedOptions.DateOnlyFormat,
            TimeOnlyFormat = timeOnlyFormat ?? AdvancedOptions.TimeOnlyFormat,
            TimeSpanFormat = timeSpanFormat ?? AdvancedOptions.TimeSpanFormat,
        };

        var result = this with
        {
            AdvancedOptions = advanced,
            StringifyAs = stringifyAs ?? StringifyAs,
        };

        if (writeIndented.HasValue)
            result.WriteIndented = writeIndented.Value;

        return result;
    }

    // ──────────────────────────────────────────────
    //  Implicit operators
    // ──────────────────────────────────────────────

    /// <summary>
    /// Extract the <see cref="JsonOptions"/> out of <paramref name="o"/>
    /// </summary>
    public static implicit operator JsonSerializerOptions(TooStringOptions o)
        => o.JsonOptions;

    /// <summary>
    /// Create <see cref="TooStringOptions"/> from <paramref name="jsonSerializerOptions"/>
    /// </summary>
    public static implicit operator TooStringOptions(JsonSerializerOptions jsonSerializerOptions)
        => new(AdvancedOptions.Default,
               jsonSerializerOptions,
               TooStringStyle.JsonSerializer);

    /// <summary>
    /// Create <see cref="TooStringOptions"/> from <paramref name="advancedOptions"/>
    /// </summary>
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
