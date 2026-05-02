using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace LogAssert;

// Traditional (message, params args) style — non-clashing overloads only
public static partial class LogAssertions
{
    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="message"/>
    /// with <paramref name="delayedArgs"/> evaluated lazily at the specified <paramref name="logLevel"/>.
    /// </summary>
    public static bool LogIf<T>(ILogger log,
                                LogLevel logLevel,
                                bool condition,
                                [CallerArgumentExpression("condition")] string? message = null,
                                Func<T>? delayedArgs = null)
    {
        try
        {
            if (condition) log.Log(logLevel, message, delayedArgs is null ? null : delayedArgs());
        }
        catch (Exception e)
        {
            log.LogError(e,
                message: "({Message},{@delayedArgs}) was true, but logging it threw an Exception",
                message, delayedArgs);
        }
        return condition;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="message"/>
    /// at the specified <paramref name="logLevel"/>.
    /// </summary>
    static bool LogIfClassic(ILogger log,
                             LogLevel logLevel,
                             bool condition,
                             [CallerArgumentExpression("condition")] string? message = null,
                             params object?[] args)
    {
        try
        {
            var missingPlaceholders = args.Length - (message?.Count(c => c == '{') ?? 0);
            for (int i = 0; i < missingPlaceholders; i++)
            {
                message += $":{{arg{i}}}";
            }
            if (condition) log.Log(logLevel, message, args);
        }
        catch (Exception e)
        {
            log.LogError(e, message: "({Message},{@args}) was true, but logging it threw an Exception", message, args);
        }
        return condition;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Warning"/>
    /// with lazily evaluated args.
    /// </summary>
    public static bool LogWarnIf<T>(this ILogger log,
                                    bool condition,
                                    [CallerArgumentExpression("condition")] string? message = null,
                                    Func<T>? delayedArgs = null)
        => LogIf(log, LogLevel.Warning, condition, message, delayedArgs);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Error"/>
    /// with lazily evaluated args.
    /// </summary>
    public static bool LogErrorIf<T>(this ILogger log,
                                     bool condition,
                                     [CallerArgumentExpression("condition")] string? message = null,
                                     Func<T>? delayedArgs = null)
        => LogIf(log, LogLevel.Error, condition, message, delayedArgs);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Information"/>
    /// with lazily evaluated args.
    /// </summary>
    public static bool LogInformationIf<T>(this ILogger log,
                                           bool condition,
                                           [CallerArgumentExpression("condition")] string? message = null,
                                           Func<T>? delayedArgs = null)
        => LogIf(log, LogLevel.Information, condition, message, delayedArgs);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogLevel.Debug"/>
    /// with lazily evaluated args.
    /// </summary>
    public static bool LogDebugIf<T>(this ILogger log,
                                     bool condition,
                                     [CallerArgumentExpression("condition")] string? message = null,
                                     Func<T>? delayedArgs = null)
        => LogIf(log, LogLevel.Debug, condition, message, delayedArgs);

    /// <summary>
    /// If <c>that(<paramref name="this"/>)</c> evaluates true,
    /// log at <see cref="LogLevel.Warning"/> with <paramref name="args"/>.
    /// </summary>
    /// <returns>The result of <c>that(this)</c>, or null if evaluation threw.</returns>
    public static bool? LogWarnIf<T>(this ILogger log,
                                     T @this,
                                     Func<T, bool> that,
                                     string message,
                                     params object[] args)
    {
        bool outcome;
        try
        {
            outcome = that(@this);
            if (outcome) log.LogWarning(message, args);
        }
        catch (Exception e)
        {
            log.LogError(e, message: "({Message},{@args}) threw when evaluating it.", message, args);
            return null;
        }
        return outcome;
    }

    /// <summary>
    /// If <c>that(<paramref name="this"/>)</c> evaluates true,
    /// log at <see cref="LogLevel.Warning"/> with lazily evaluated <paramref name="delayedArgs"/>.
    /// </summary>
    /// <returns>The result of <c>that(this)</c>, or null if evaluation threw.</returns>
    public static bool? LogWarnIf<T, Ta>(this ILogger log,
                                         T @this,
                                         Func<T, bool> that,
                                         string message,
                                         Func<Ta> delayedArgs)
    {
        bool outcome;
        try
        {
            outcome = that(@this);
            if (outcome) log.LogWarning(message, delayedArgs());
        }
        catch (Exception e)
        {
            log.LogError(e, message: "({Message},{delayedArgs}) threw when evaluating it.", message, delayedArgs);
            return null;
        }
        return outcome;
    }

    // ── LogIfNot family ─────────────────────────────────────────────────

    /// <summary>
    /// If <paramref name="condition"/> is false, log at <see cref="LogLevel.Warning"/>.
    /// </summary>
    public static bool LogWarnIfNot(this ILogger log, bool condition, string message, params object[] args)
        => LogIfNot(log, LogLevel.Warning, condition, message, args);

    /// <summary>
    /// If <paramref name="condition"/> is false, log at <see cref="LogLevel.Error"/>.
    /// </summary>
    public static bool LogErrorIfNot(this ILogger log, bool condition, string message, params object[] args)
        => LogIfNot(log, LogLevel.Error, condition, message, args);

    /// <summary>
    /// If <paramref name="condition"/> is false, log at <see cref="LogLevel.Information"/>.
    /// </summary>
    public static bool LogInformationIfNot(this ILogger log, bool condition, string message, params object[] args)
        => LogIfNot(log, LogLevel.Information, condition, message, args);

    static bool LogIfNot(ILogger log, LogLevel logLevel, bool condition, string message, object[] args)
    {
        try
        {
            if (!condition) log.Log(logLevel, message, args);
        }
        catch (Exception e)
        {
            log.LogError(e, message: "({Message},{@args}) was false, but logging it threw an Exception", message, args);
        }
        return condition;
    }

    /// <summary>
    /// If <c>that(<paramref name="this"/>)</c> evaluates false,
    /// log at <see cref="LogLevel.Warning"/> with <paramref name="args"/>.
    /// </summary>
    /// <returns>The result of <c>that(this)</c>, or null if evaluation threw.</returns>
    public static bool? LogWarnIfNot<T>(this ILogger log,
                                        T @this,
                                        Func<T, bool> that,
                                        string message,
                                        params object[] args)
    {
        bool outcome;
        try
        {
            outcome = that(@this);
            if (!outcome) log.LogWarning(message, args);
        }
        catch (Exception e)
        {
            log.LogError(e, message: "({Message},{@args}) threw when evaluating it.", message, args);
            return null;
        }
        return outcome;
    }
}
