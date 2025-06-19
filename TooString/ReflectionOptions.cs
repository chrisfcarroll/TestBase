using System.Reflection;

namespace TooString;

/// <summary>
/// Options for what to stringify when using <see cref="TooStringHow.Reflection"/>
/// /// </summary>
/// <param name="WhichProperties"><see cref="BindingFlags"/> to pick out the properties and fields to stringify</param>
/// <param name="Style">Defaults to <see cref="ReflectionStyle.Json"/></param>
/// <param name="MaxDepth">Defaults to 3, which is low for serialization.</param>
/// <param name="DateTimeFormat">Defaults to O. The preferred <see cref="DateTime.ToString()"/> option</param>
/// <param name="DateOnlyFormat">Defaults to O. The preferred <see cref="DateOnly.ToString()"/> option</param>
/// <param name="TimeOnlyFormat">Defaults to O. The preferred <see cref="TimeOnly.ToString()"/> option</param>
/// <param name="TimeSpanFormat">Defaults to "". The preferred <see cref="TimeSpan.ToString()"/> option</param>
public record ReflectionOptions(
    BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    ReflectionStyle Style = ReflectionStyle.Json,
    int MaxDepth = 3,
    string DateTimeFormat = "O",
    string DateOnlyFormat = "O",
    string TimeOnlyFormat = "HH:mm:ss",
    string TimeSpanFormat = "c")
{
    /// <summary>
    /// Default instance is
    /// <code>
    /// public record ReflectionOptions(
    /// BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    ///     ReflectionStyle Style = ReflectionStyle.Json,
    ///     int MaxDepth = 3,
    ///     string DateTimeFormat="O")
    /// </code>
    /// </summary>
    public static readonly ReflectionOptions Default = new();
}