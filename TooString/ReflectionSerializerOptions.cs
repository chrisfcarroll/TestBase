using System.Reflection;

namespace TooString;

/// <summary>
/// Options for what to serialize when using <see cref="TooStringStyle.Reflection"/>
/// </summary>
/// <param name="WhichProperties"></param>
/// <param name="Style"></param>
/// <param name="MaxDepth"></param>
public record ReflectionSerializerOptions(
    BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    SerializationStyle Style = SerializationStyle.Json,
    int MaxDepth = 3
)
{
    public static readonly ReflectionSerializerOptions Default = new();
}