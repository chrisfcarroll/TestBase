using System.Text.Json;

namespace TooString;

/// <summary>
/// <see cref="With{T}"/> and <see cref="With"/> and <see cref="AddAll{T}"/>
/// </summary>
public static class WithExtension
{
    /// <summary>
    /// A mutating <c>with</c>
    /// </summary>
    /// <param name="this">The thing to mutate</param>
    /// <param name="with">The mutations to apply</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>The passed-in <paramref name="this"/></returns>
    public static T With<T>(this T @this, Action<T> with)
    {
        with(@this);
        return @this;
    }

    /// <summary>
    /// Return a copy of <paramref name="this"/> with the changes
    /// specified in <paramref name="with"/>
    /// </summary>
    /// <param name="this"></param>
    /// <param name="with"></param>
    /// <returns>a new <see cref="JsonSerializerOptions"/></returns>
    public static JsonSerializerOptions With(
        this JsonSerializerOptions @this,
        Action<JsonSerializerOptions> with)
    {
        var copy = new JsonSerializerOptions
        {
            PropertyNamingPolicy = @this.PropertyNamingPolicy,
            #pragma warning disable SYSLIB0020
            IgnoreNullValues = @this.IgnoreNullValues,
            #pragma warning restore SYSLIB0020
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
            PropertyNameCaseInsensitive = @this.PropertyNameCaseInsensitive,
            #if NET8_0_OR_GREATER
            TypeInfoResolver = @this.TypeInfoResolver,
            UnmappedMemberHandling = @this.UnmappedMemberHandling,
            PreferredObjectCreationHandling = @this.PreferredObjectCreationHandling,
            #endif
        };
        copy.Converters.AddAll(@this.Converters);
        with(copy);
        return copy;
    }

    /// <summary>
    /// Adds each of <paramref name="other"/> to <paramref name="this"/>
    /// </summary>
    /// <param name="this"></param>
    /// <param name="other"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IList<T> AddAll<T>(this IList<T> @this, IEnumerable<T> other)
    {
        foreach (var item in other)
        {
            @this.Add(item);
        }
        return @this;
    }
}