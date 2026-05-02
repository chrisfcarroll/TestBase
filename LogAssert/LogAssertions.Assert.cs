using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace LogAssert;

// Assert, AssertNotNull, LogAndThrowIfNot, LogAndThrowIfNull
public static partial class LogAssertions
{
    /// <summary>
    /// If <paramref name="assertion"/> is false, log at <see cref="LogLevel.Error"/>.
    /// </summary>
    public static void Assert(this ILogger log,
                              bool assertion,
                              object? loggableState = null,
                              [CallerMemberName] string action = "",
                              [CallerArgumentExpression("assertion")] string? assertionExpression = "",
                              [CallerArgumentExpression("loggableState")] string? stateName = "")
    {
        if (assertion) return;
        log.LogError("{Action}:AssertionFailed:{Assertion}:{StateName}{LoggableState}",
                     action,
                     assertionExpression,
                     StateNameIfHelpful(action, stateName),
                     loggableState ?? string.Empty);
    }
    
    /// <summary>
    /// If <paramref name="precondition"/> is false, log at <see cref="LogLevel.Error"/>.
    /// </summary>
    public static void PreCondition(this ILogger log,
                                    bool precondition,
                                    object? loggableState = null,
                                    [CallerMemberName] string action = "",
                                    [CallerArgumentExpression("precondition")] string? assertionExpression = "",
                                    [CallerArgumentExpression("loggableState")] string? stateName = "")
    {
        if (precondition) return;
        log.LogError("{Action}:PreConditionFailed:{Assertion}:{StateName}{LoggableState}",
                     action,
                     assertionExpression,
                     StateNameIfHelpful(action, stateName),
                     loggableState ?? string.Empty);
    }    
    /// <summary>
    /// If <paramref name="postcondition"/> is false, log at <see cref="LogLevel.Error"/>.
    /// </summary>
    public static void PostCondition(this ILogger log,
                                    bool postcondition,
                                    object? loggableState = null,
                                    [CallerMemberName] string action = "",
                                    [CallerArgumentExpression("postcondition")] string? assertionExpression = "",
                                    [CallerArgumentExpression("loggableState")] string? stateName = "")
    {
        if (postcondition) return;
        log.LogError("{Action}:PostConditionFailed:{Assertion}:{StateName}{LoggableState}",
                     action,
                     assertionExpression,
                     StateNameIfHelpful(action, stateName),
                     loggableState ?? string.Empty);
    }    
    

    /// <summary>
    /// Log an error if <paramref name="it"/> is null.
    /// This is not fail-fast — execution continues.
    /// For fail-fast use <see cref="LogAndThrowIfNull{T}(ILogger,T?,string,object[])"/>.
    /// </summary>
    /// <returns>true if <paramref name="it"/> is not null.</returns>
    public static bool AssertNotNull<T>(this ILogger log,
                                        [NotNull] T? it,
                                        object? loggableState = null,
                                        [CallerArgumentExpression("it")] string? expr = null)
    {
        log.LogErrorIf(it is null, loggableState.ToLoggableState()?.ToString() ?? ($"{expr} is null"));
        #pragma warning disable CS8777
        return it is not null;
        #pragma warning restore CS8777
    }

    // ── LogAndThrowIfNot ────────────────────────────────────────────────

    /// <summary>
    /// If <paramref name="condition"/> is false, log as Error and throw <see cref="InvalidOperationException"/>.
    /// </summary>
    public static bool LogAndThrowIfNot(this ILogger log,
                                        bool condition,
                                        string message,
                                        params object[] args)
    {
        try
        {
            if (condition) return condition;
            InvalidOperationException exception = new(message: PoorFormat(message, args));
            log.LogError(exception, message, args);
            throw exception;
        }
        catch (Exception e)
        {
            log.LogError(e, MessageThrew, args);
            throw;
        }
    }

    /// <summary>
    /// If <paramref name="condition"/> is false, log as Error and throw <paramref name="exception"/>.
    /// </summary>
    public static bool LogAndThrowIfNot(this ILogger log,
                                        bool condition,
                                        Exception exception,
                                        params object[] args)
    {
        try
        {
            if (condition) return condition;
            log.LogError(exception, MessageWasFalse, args);
            throw exception;
        }
        catch (Exception e)
        {
            log.LogError(e, MessageThrew, args);
            throw;
        }
    }

    /// <summary>
    /// If <paramref name="condition"/> is false, log as Error and throw the result of <paramref name="exception"/>.
    /// </summary>
    public static bool LogAndThrowIfNot(this ILogger log,
                                        bool condition,
                                        Func<Exception> exception,
                                        params object[] args)
    {
        try
        {
            if (condition) return condition;
            var ex = exception();
            log.LogError(ex, MessageWasFalse, args);
            throw ex;
        }
        catch (Exception e)
        {
            log.LogError(e, MessageThrew, args);
            throw;
        }
    }

    /// <summary>
    /// If <paramref name="that"/> is false, log as Error and throw the result of <paramref name="exception"/>.
    /// <paramref name="delayedArgs"/> is only evaluated when <paramref name="that"/> is false.
    /// </summary>
    public static bool LogAndThrowIfNot(this ILogger log,
                                        bool that,
                                        Func<Exception> exception,
                                        Func<object[]> delayedArgs)
    {
        try
        {
            if (that) return that;
            var ex = exception();
            log.LogError(ex, MessageWasFalse, args: delayedArgs());
            throw ex;
        }
        catch (Exception e)
        {
            log.LogError(e, MessageThrew, "(Func<object[]> delayedArgs not evaluated)");
            throw;
        }
    }

    // ── LogAndThrowIfNull ───────────────────────────────────────────────

    /// <summary>
    /// If <paramref name="it"/> is null, log as Error and throw <see cref="ArgumentNullException"/>.
    /// </summary>
    [return: NotNull]
    public static T LogAndThrowIfNull<T>(this ILogger log,
                                         T? it,
                                         string message,
                                         params object[] args)
    {
        try
        {
            if (it is not null) return it;
            ArgumentNullException exception = new(paramName: PoorFormat(message, args));
            log.LogError(exception, message, args);
            throw exception;
        }
        catch (Exception e)
        {
            log.LogError(e, MessageThrew, args);
            throw;
        }
    }

    /// <summary>
    /// If <paramref name="it"/> is null, log as Error and throw <paramref name="exception"/>.
    /// </summary>
    [return: NotNull]
    public static T LogAndThrowIfNull<T>(this ILogger log,
                                         [NotNull] T? it,
                                         Exception exception,
                                         params object[] args)
    {
        try
        {
            if (it is not null) return it;
            log.LogError(exception, MessageWasFalse, args);
            throw exception;
        }
        catch (Exception e)
        {
            log.LogError(e, MessageThrew, args);
            throw;
        }
    }

    /// <summary>
    /// If <paramref name="it"/> is null, log as Error and throw the result of <paramref name="exception"/>.
    /// </summary>
    [return: NotNull]
    public static T LogAndThrowIfNull<T>(this ILogger log,
                                         [NotNull] T? it,
                                         Func<Exception> exception,
                                         params object[] args)
    {
        try
        {
            if (it is not null) return it;
            var ex = exception();
            log.LogError(ex, MessageWasFalse, args);
            throw ex;
        }
        catch (Exception e)
        {
            log.LogError(e, MessageThrew, args);
            throw;
        }
    }

    /// <summary>
    /// If <paramref name="it"/> is null, log as Error and throw the result of <paramref name="exception"/>.
    /// <paramref name="delayedArgs"/> is only evaluated when <paramref name="it"/> is null.
    /// </summary>
    [return: NotNull]
    public static T LogAndThrowIfNull<T>(this ILogger log,
                                         [NotNull] T? it,
                                         Func<Exception> exception,
                                         Func<object[]> delayedArgs)
    {
        try
        {
            if (it is not null) return it;
            var ex = exception();
            log.LogError(ex, MessageWasFalse, args: delayedArgs());
            throw ex;
        }
        catch (Exception e)
        {
            log.LogError(e, MessageThrew, "(Func<object[]> delayedArgs not evaluated)");
            throw;
        }
    }
}
