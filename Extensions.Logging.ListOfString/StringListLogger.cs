using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Extensions.Logging.ListOfString
{
    public static class StringListLoggerFactoryExtension
    {
        /// <summary>
        ///     Ensures that when a <see cref="ILogger" /> is requested it will write to <see cref="StringListLogger.Instance" />
        /// </summary>
        /// <param name="backingList">
        /// If specified, the returned <see cref="ILogger" /> will write to <paramref name="backingList" />
        /// </param>
        /// <param name="includeScopes"></param>
        /// <param name="filter"></param>
        /// <returns>the <paramref name="factory"/></returns>
        public static ILoggerFactory AddStringListLogger(
            this ILoggerFactory          factory,
            List<string>                 backingList,
            bool                         includeScopes = true,
            Func<string, LogLevel, bool> filter        = null)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            filter = filter ?? ((s, t) => true);
            var soleInstance = new StringListLogger(backingList, "", filter, includeScopes);
            factory.AddProvider(new StringListLoggerProvider(soleInstance));
            return factory;
        }

        /// <summary>
        /// Ensures that when a <see cref="ILogger" /> is requested it will write to <paramref name="providerInstance"/>.
        /// Use this in preference to <see cref="AddStringListLogger(ILoggerFactory,List{string},bool,Func{string,LogLevel,bool})"/>
        /// if your test suite uses multiple <see cref="ILoggerFactory"/>s in parallel.
        /// </summary>
        /// <param name="providerInstance"></param>
        /// <remarks>If this method is called multiple times, the global
        /// <see cref="StringListLogger"/>.<see cref="StringListLogger.Instance"/>
        /// will be overwritten each time by the last provided <paramref name="providerInstance"/>.
        /// </remarks>
        /// <returns>the <paramref name="factory"/></returns>
        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory, StringListLogger providerInstance)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            factory.AddProvider(new StringListLoggerProvider(providerInstance));
            return factory;
        }

        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory)
        {
            return AddStringListLogger(factory, new StringListLogger());
        }
    }

    /// <summary>
    /// A <see cref="ILoggerProvider"/> which returns <see cref="Instance"/> each time
    /// <see cref="CreateLogger"/> is called.
    /// </summary>
    public class StringListLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// A <b>per-provider</b> instance of a <see cref="StringListLogger"/>.
        /// Use this if your test suite creates multiple <see cref="StringListLoggerProvider"/>s
        /// in parallel, to examine the log lines from each
        /// independent <see cref="StringListLoggerProvider"/>.
        /// </summary>
        public StringListLogger Instance;

        readonly Stack<string> names = new Stack<string>();

        /// <summary>
        /// Create a new <see cref="StringListLoggerProvider"/> which has <see cref="Instance"/>
        /// set to the given <see cref="StringListLogger"/>.
        ///
        /// <p><b>Side-Effect</b>This constructor also sets <see cref="StringListLogger"/>.
        /// <see cref="StringListLogger.Instance"/> to the given <paramref name="instance"/>.</p>
        /// You can avoid this by using the parameterless <see cref="StringListLoggerProvider()"/>
        /// constructorinstead, which preserves a single
        /// <see cref="StringListLogger"/>.<see cref="StringListLogger.Instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        public StringListLoggerProvider(StringListLogger instance) { StringListLogger.Instance = Instance = instance; }

        /// <summary>
        /// Create a new <see cref="StringListLoggerProvider"/> which has <see cref="Instance"/>
        /// set to the global singleton <see cref="StringListLogger"/>.<see cref="StringListLogger.Instance"/>
        /// </summary>
        /// <param name="instance"></param>
        public StringListLoggerProvider() : this(StringListLogger.Instance) { }

        public void Dispose() { Instance.Name = names.Pop(); }

        public ILogger CreateLogger(string categoryName)
        {
            names.Push(Instance.Name = categoryName);
            return Instance;
        }
    }

    /// <summary>An <see cref="ILogger" /> which will log to its <see cref="LoggedLines" />.</summary>
    public class StringListLogger : ILogger
    {
        /// <summary>
        /// <p>A singleton instance of <see cref="StringListLogger" /> which can be used for
        /// testing logging in classes dependent on <see cref="ILogger" />.</p>
        /// <p>Use <c>StringListLogger.Instance.LoggedLines</c> to inspect or
        /// assert on logged lines.</p>
        /// <p>This instance will also be returned by a <see cref="StringListLoggerProvider" />
        /// created with the parameterless constructor, and by extension from any
        /// <see cref="ILoggerFactory" /> which has called
        /// <see
        ///      cref="StringListLoggerFactoryExtension.AddStringListLogger(ILoggerFactory,List{string},bool,Func{string,LogLevel,bool})" />
        /// </p>
        /// </summary>
        /// <remarks>
        /// Do not make use of this instance in parallel-running test suites using multiple
        /// <see cref="ILoggerFactory" />s creating multiple <see cref="StringListLoggerProvider" />s
        /// because this Instance will be overwritten by the last <see cref="StringListLoggerProvider"/>
        /// created. Instead, inspect or assert each <see cref="StringListLoggerProvider"/>'s own
        /// <see cref="StringListLoggerProvider.Instance"/> instead of this global instance.
        /// </remarks>
        public static StringListLogger Instance = new StringListLogger();

        static readonly string LoglevelPadding = ": ";

        static readonly string MessagePadding =
        new string(' ', LogLevel.Information.ToString().Length + LoglevelPadding.Length);

        static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;

        [ThreadStatic] static StringBuilder logBuilder;
        Func<string, LogLevel, bool> filter;

        public StringListLogger(
            List<string>                 backingList   = null,
            string                       name          = null,
            Func<string, LogLevel, bool> filter        = null,
            bool                         includeScopes = true)
        {
            Name          = name   ?? string.Empty;
            Filter        = filter ?? ((category, logLevel) => true);
            IncludeScopes = includeScopes;
            LoggedLines   = backingList ?? new List<string>();
        }

        /// <summary>
        ///     The record of lines logged to this <see cref="StringListLogger" />
        ///     If the <see cref="StringListLogger" /> was created by passing in an existing <see cref="List{String}" />, then this
        ///     property will be <seealso cref="object.ReferenceEquals" />() to that list.
        /// </summary>
        public List<string> LoggedLines { get; }

        public Func<string, LogLevel, bool> Filter
        {
            protected internal get => filter;
            set => filter = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool IncludeScopes { get; set; }

        public string Name { get; set; }

        public ScopeStack Scopes { get; } = new ScopeStack();

        public void Log<TState>(
            LogLevel                        logLevel,
            EventId                         eventId,
            TState                          state,
            Exception                       exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            string message;
            try { message = formatter(state, exception); } catch (Exception e)
            {
                message = string.Format("error trying to log {0} {1}\nException: {2}", state?.GetType(), exception, e);
            }

            if (string.IsNullOrEmpty(message) && exception == null) return;
            WriteMessage(logLevel, Name, eventId.Id, message, exception);
        }

        public bool IsEnabled(LogLevel logLevel) { return Filter(Name, logLevel); }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            Scopes.Push(new Tuple<string, object>(Name, state));
            return Scopes;
        }

        public virtual void WriteMessage(
            LogLevel  logLevel,
            string    logName,
            int       eventId,
            string    message,
            Exception exception)
        {
            var builder = logBuilder;
            logBuilder = null;
            if (builder == null) builder = new StringBuilder();
            builder.Append(LoglevelPadding);
            builder.Append(logName);
            builder.Append("[");
            builder.Append(eventId);
            builder.AppendLine("]");
            if (IncludeScopes) GetScopeInformation(builder);
            if (!string.IsNullOrEmpty(message))
            {
                builder.Append(MessagePadding);
                var length = builder.Length;
                builder.AppendLine(message);
                builder.Replace(Environment.NewLine, NewLineWithMessagePadding, length, message.Length);
            }

            if (exception      != null) builder.AppendLine(exception.ToString());
            if (builder.Length > 0) LoggedLines.Add($"[{logLevel.ToString()}] {builder}");

            builder.Clear();
            if (builder.Capacity > 1024) builder.Capacity = 1024;
            logBuilder = builder;
        }

        void GetScopeInformation(StringBuilder builder)
        {
            var length = builder.Length;
            foreach (var scope in Scopes)
            {
                var asString = scope.Item2 is Type t ? t.Name : scope.Item2;
                var str = length != builder.Length
                          ? string.Format("=> {0} ", asString)
                          : string.Format("=> {0}",  asString);
                builder.Insert(length, str);
            }

            if (builder.Length <= length) return;
            builder.Insert(length, MessagePadding);
            builder.AppendLine();
        }

        public class ScopeStack : Stack<Tuple<string, object>>, IDisposable
        {
            public void Dispose() { Pop(); }

            public new ScopeStack Push(Tuple<string, object> item)
            {
                base.Push(item);
                return this;
            }
        }
    }
}
