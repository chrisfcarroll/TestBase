using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;

namespace SerilogAssert;

// Traditional (message, params args) style — non-clashing overloads only
public static partial class SerilogAssertions
{
    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="message"/>
    /// with <paramref name="delayedArgs"/> evaluated lazily at the specified <paramref name="logLevel"/>.
    /// </summary>
    public static bool If<T>(ILogger log,
                             LogEventLevel logLevel,
                             bool condition,
                             [CallerArgumentExpression("condition")] string? message = null,
                             Func<T>? delayedArgs = null)
    {
        try
        {
            if (condition) log.Write(logLevel, message!, delayedArgs is null ? null : delayedArgs());
        }
        catch (Exception e)
        {
            log.Error(e,
                "({Message},{@delayedArgs}) was true, but logging it threw an Exception",
                message, delayedArgs);
        }
        return condition;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log <paramref name="message"/>
    /// at the specified <paramref name="logLevel"/>.
    /// </summary>
    static bool IfClassic(ILogger log,
                          LogEventLevel logLevel,
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
            if (condition) log.Write(logLevel, message!, args);
        }
        catch (Exception e)
        {
            log.Error(e, "({Message},{@args}) was true, but logging it threw an Exception", message, args);
        }
        return condition;
    }

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Warning"/>
    /// with lazily evaluated args.
    /// </summary>
    public static bool WarnIf<T>(this ILogger log,
                                 bool condition,
                                 [CallerArgumentExpression("condition")] string? message = null,
                                 Func<T>? delayedArgs = null)
        => If(log, LogEventLevel.Warning, condition, message, delayedArgs);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Error"/>
    /// with lazily evaluated args.
    /// </summary>
    public static bool ErrorIf<T>(this ILogger log,
                                  bool condition,
                                  [CallerArgumentExpression("condition")] string? message = null,
                                  Func<T>? delayedArgs = null)
        => If(log, LogEventLevel.Error, condition, message, delayedArgs);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Information"/>
    /// with lazily evaluated args.
    /// </summary>
    public static bool InformationIf<T>(this ILogger log,
                                        bool condition,
                                        [CallerArgumentExpression("condition")] string? message = null,
                                        Func<T>? delayedArgs = null)
        => If(log, LogEventLevel.Information, condition, message, delayedArgs);

    /// <summary>
    /// If <paramref name="condition"/> is true, log at <see cref="LogEventLevel.Debug"/>
    /// with lazily evaluated args.
    /// </summary>
    public static bool DebugIf<T>(this ILogger log,
                                  bool condition,
                                  [CallerArgumentExpression("condition")] string? message = null,
                                  Func<T>? delayedArgs = null)
        => If(log, LogEventLevel.Debug, condition, message, delayedArgs);

    /// <summary>
    /// If <c>that(<paramref name="this"/>)</c> evaluates true,
    /// log at <see cref="LogEventLevel.Warning"/> with <paramref name="args"/>.
    /// </summary>
    /// <returns>The result of <c>that(this)</c>, or null if evaluation threw.</returns>
    public static bool? WarnIf<T>(this ILogger log,
                                  T @this,
                                  Func<T, bool> that,
                                  string message,
                                  params object[] args)
    {
        bool outcome;
        try
        {
            outcome = that(@this);
            if (outcome) log.Warning(message, args);
        }
        catch (Exception e)
        {
            log.Error(e, "({Message},{@args}) threw when evaluating it.", message, args);
            return null;
        }
        return outcome;
    }

    /// <summary>
    /// If <c>that(<paramref name="this"/>)</c> evaluates true,
    /// log at <see cref="LogEventLevel.Warning"/> with lazily evaluated <paramref name="delayedArgs"/>.
    /// </summary>
    /// <returns>The result of <c>that(this)</c>, or null if evaluation threw.</returns>
    public static bool? WarnIf<T, Ta>(this ILogger log,
                                      T @this,
                                      Func<T, bool> that,
                                      string message,
                                      Func<Ta> delayedArgs)
    {
        bool outcome;
        try
        {
            outcome = that(@this);
            if (outcome) log.Warning(message, delayedArgs());
        }
        catch (Exception e)
        {
            log.Error(e, "({Message},{delayedArgs}) threw when evaluating it.", message, delayedArgs);
            return null;
        }
        return outcome;
    }

    // ── IfNot family ───────────────────────────────────────────────────

    /// <summary>
    /// If <paramref name="condition"/> is false, log at <see cref="LogEventLevel.Warning"/>.
    /// </summary>
    public static bool WarnIfNot(this ILogger log, bool condition, string message, params object[] args)
        => IfNot(log, LogEventLevel.Warning, condition, message, args);

    /// <summary>
    /// If <paramref name="condition"/> is false, log at <see cref="LogEventLevel.Error"/>.
    /// </summary>
    public static bool ErrorIfNot(this ILogger log, bool condition, string message, params object[] args)
        => IfNot(log, LogEventLevel.Error, condition, message, args);

    /// <summary>
    /// If <paramref name="condition"/> is false, log at <see cref="LogEventLevel.Information"/>.
    /// </summary>
    public static bool InformationIfNot(this ILogger log, bool condition, string message, params object[] args)
        => IfNot(log, LogEventLevel.Information, condition, message, args);

    static bool IfNot(ILogger log, LogEventLevel logLevel, bool condition, string message, object[] args)
    {
        try
        {
            if (!condition) log.Write(logLevel, message, args);
        }
        catch (Exception e)
        {
            log.Error(e, "({Message},{@args}) was false, but logging it threw an Exception", message, args);
        }
        return condition;
    }

    /// <summary>
    /// If <c>that(<paramref name="this"/>)</c> evaluates false,
    /// log at <see cref="LogEventLevel.Warning"/> with <paramref name="args"/>.
    /// </summary>
    /// <returns>The result of <c>that(this)</c>, or null if evaluation threw.</returns>
    public static bool? WarnIfNot<T>(this ILogger log,
                                     T @this,
                                     Func<T, bool> that,
                                     string message,
                                     params object[] args)
    {
        bool outcome;
        try
        {
            outcome = that(@this);
            if (!outcome) log.Warning(message, args);
        }
        catch (Exception e)
        {
            log.Error(e, "({Message},{@args}) threw when evaluating it.", message, args);
            return null;
        }
        return outcome;
    }
}
