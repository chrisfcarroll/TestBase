using System.Collections.Concurrent;
using System.Reflection;

namespace Serilog.Assert;

/// <summary>
/// An optional interface to remind you to add
/// a ToLoggableState() method to your class, to summarize useful state and to
/// mask or omit sensitive information.
/// </summary>
public interface ILoggable
{
    /// <summary>
    /// Return something that usefully logs your object, and masks
    /// any sensitive information.
    /// </summary>
    public object? ForLogging();
}

public static class LoggableState
{
    /// <summary>
    /// Returns something that is loggable. For ValueTypes, returns the value itself.
    /// Otherwise, returns ToString().
    ///
    /// "Override" this fallback method by defining public object ToLoggableState()
    /// on your classes, and optionally declaring them to implement ILoggable.
    /// </summary>
    public static object? ForLogging<T>(this T state)
    {
        return state switch
        {
            null => null,
            _ when TryInvokeInstanceToLoggableState(state, out var loggable) => loggable,
            ValueType => state,
            _ => state.ToString()
        };
    }

    static readonly ConcurrentDictionary<Type, MethodInfo?> ToLoggableStateMethodCache = new();

    static bool TryInvokeInstanceToLoggableState(object obj, out object? loggable)
    {
        var type = obj.GetType();

        var method = ToLoggableStateMethodCache.GetOrAdd(type, static t =>
        {
            var mi = t.GetMethod(
                "ToLoggableState",
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);

            return mi is not null && mi.ReturnType != typeof(void) ? mi : null;
        });

        if (method is null)
        {
            loggable = null;
            return false;
        }

        loggable = method.Invoke(obj, parameters: null);
        return true;
    }
}
