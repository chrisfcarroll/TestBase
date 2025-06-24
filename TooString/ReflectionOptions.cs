using System.Reflection;

namespace TooString;

/// <summary>
/// Options for what to stringify when using <see cref="TooStringHow.Reflection"/>
/// /// </summary>
/// <param name="WhichProperties"><see cref="BindingFlags"/> to pick out the properties and fields to stringify</param>
/// <param name="Style">Defaults to <see cref="ReflectionStyle.Json"/></param>
/// <param name="MaxDepth">
/// How deep into nested structures should we print before stopping the recursion?
/// Defaults to 3, which may be plenty for logging and monitoring.</param>
/// <param name="MaxLength">
/// How many elements of an Array or other IEnumerable should we print before stopping the loop?
/// Defaults to 3.
/// <p><b>NB MaxLength = 0 does not mean carry on for ever</b>. Use MaxLength = int.MaxValue for that.
/// MaxLength = 0 means, don't print any elements of an enumerable.</p>
/// <p><b>Negative MaxLength</b> will start at the positive length given, then count down as depth is descended.
/// for instance, MaxLength=-2 will use a MaxLength of 2 at depth 1, but of zero at depth 3.</p>
/// </param>
/// <param name="DateTimeFormat">Defaults to O. The preferred <see cref="DateTime.ToString()"/> option</param>
/// <param name="DateOnlyFormat">Defaults to O. The preferred <see cref="DateOnly.ToString()"/> option</param>
/// <param name="TimeOnlyFormat">Defaults to O. The preferred <see cref="TimeOnly.ToString()"/> option</param>
/// <param name="TimeSpanFormat">Defaults to "". The preferred <see cref="TimeSpan.ToString()"/> option</param>
public record ReflectionOptions(
    BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    ReflectionStyle Style = ReflectionStyle.DebugView,
    int MaxDepth = 3,
    int MaxLength = 9,
    string DateTimeFormat = "O",
    string DateOnlyFormat = "O",
    string TimeOnlyFormat = "HH:mm:ss",
    string TimeSpanFormat = "c")
{
    /// <summary>
    /// Default instance is
    /// <code>
    /// public record ReflectionOptions(
    ///     BindingFlags WhichProperties = BindingFlags.Instance | BindingFlags.Public,
    ///     ReflectionStyle Style = ReflectionStyle.DebugView,
    ///     int MaxDepth = 3,
    ///     int MaxLength = 9,
    ///     string DateTimeFormat = "O",
    ///     string DateOnlyFormat = "O",
    ///     string TimeOnlyFormat = "HH:mm:ss",
    ///     string TimeSpanFormat = "c")
    /// </code>
    /// </summary>
    public static readonly ReflectionOptions ForDebugView = new();

    /// <summary>
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
    public static readonly ReflectionOptions ForJson = ForDebugView with
    {
        Style = ReflectionStyle.Json
    };

}