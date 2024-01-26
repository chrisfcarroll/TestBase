using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TooString;

public record TooStringOptions(
    IEnumerable<TooStringMethod> PreferenceOrder,
    JsonSerializerOptions JsonOptions,
    ReflectionSerializerOptions ReflectionOptions,
    JsonSerializerDefaults JsonDefaults = JsonSerializerDefaults.General)
{
    /// <summary><c>
    /// new(JsonSerializerDefaults.Web)
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
    ///     PreferenceOrder:[TooStringMethod.ArgumentExpression, TooStringMethod.SystemTextJson],
    ///     JsonSerializerOptions:DefaultJsonOptions
    ///     ReflectionSerializerOptions:ReflectionSerializerOptions.Default,
    ///     JsonSerializerDefaults:JsonSerializerDefaults.Web,
    /// </c>
    /// </summary>
    public static readonly TooStringOptions Default =
        new(
            ((List<TooStringMethod>)
                [TooStringMethod.CallerArgument, TooStringMethod.SystemTextJson]
            ).AsReadOnly(),
            DefaultJsonOptions,
            ReflectionSerializerOptions.Default,
            JsonSerializerDefaults.General
        );

    public static implicit operator JsonSerializerOptions(TooStringOptions o)
        => o.JsonOptions;
}

public record ReflectionSerializerOptions(
    BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    SerializationStyle Style = SerializationStyle.Json,
    int MaxDepth = 1
)
{
    public static readonly ReflectionSerializerOptions Default = new();
}

public enum SerializationStyle
{
    Json = 0,
    CSharp = 1
}

internal record
    OptionsWithState(
        int Depth,
        IEnumerable<TooStringMethod> PreferenceOrder,
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