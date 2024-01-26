using System.Text.Json;

namespace TooString;

public static class Extensions
{
    public static T With<T>(this T @this, Action<T> with)
    {
        with(@this);
        return @this;
    }
    public static JsonSerializerOptions With(
        this JsonSerializerOptions @this, 
        Action<JsonSerializerOptions> with)
    {
        var copy = new JsonSerializerOptions
        {
            PropertyNamingPolicy = @this.PropertyNamingPolicy,
            IgnoreNullValues = @this.IgnoreNullValues,
            IgnoreReadOnlyProperties = @this.IgnoreReadOnlyProperties,
            WriteIndented = @this.WriteIndented,
            MaxDepth = @this.MaxDepth,
            Encoder = @this.Encoder,
            IncludeFields = @this.IncludeFields,
            NumberHandling = @this.NumberHandling,
            ReferenceHandler = @this.ReferenceHandler,
            AllowTrailingCommas = @this.AllowTrailingCommas,
            DefaultBufferSize = @this.DefaultBufferSize,
            DefaultIgnoreCondition = @this.DefaultIgnoreCondition,
            DictionaryKeyPolicy = @this.DictionaryKeyPolicy,
            ReadCommentHandling = @this.ReadCommentHandling,
            UnknownTypeHandling = @this.UnknownTypeHandling,
            IgnoreReadOnlyFields = @this.IgnoreReadOnlyFields,
            PropertyNameCaseInsensitive = @this.PropertyNameCaseInsensitive
        };
        copy.Converters.AddAll(@this.Converters);
        with(copy);
        return copy;
    }

    public static IList<T> AddAll<T>(this IList<T> @this, IEnumerable<T> other)
    {
        foreach (var item in other)
        {
            @this.Add(item);
        }
        return @this;
    }
}