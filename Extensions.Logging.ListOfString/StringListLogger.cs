using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Extensions.Logging.ListOfString
{
    public static class StringListLoggerFactoryExtension
    {
        /// <summary>
        /// Ensures that when a <see cref="ILogger"/> is requested it will write to <see cref="StringListLogger.Instance"/>
        /// </summary>
        /// <param name="backingList">If specified, the returned <see cref="ILogger"/> will write to <paramref name="backingList"/></param>
        /// <param name="includeScopes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory, List<string> backingList, bool includeScopes = true, Func<string, LogLevel, bool> filter = null)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            filter = filter ?? ((s,t)=>true);
            var soleInstance= new StringListLogger(backingList, "", filter, includeScopes);
            factory.AddProvider(new StringListLoggerProvider(soleInstance));
            return factory;
        }

        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory, StringListLogger soleInstance)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            factory.AddProvider(new StringListLoggerProvider(soleInstance));
            return factory;
        }
        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory)
        {
            return AddStringListLogger(factory, new StringListLogger());
        }
    }

    public class StringListLoggerProvider : ILoggerProvider
    {
        Stack<string> names= new Stack<string>();

        public StringListLoggerProvider(StringListLogger instance){StringListLogger.Instance =   instance; }

        public StringListLoggerProvider(): this(new StringListLogger()){}

        public void Dispose(){ StringListLogger.Instance.Name=names.Pop();} 

        public ILogger CreateLogger(string categoryName)
        {
            names.Push( StringListLogger.Instance.Name = categoryName) ;
            return StringListLogger.Instance;
        }
    }

    /// <summary>An <see cref="ILogger"/> which will log to its <see cref="LoggedLines"/>.</summary>
    public class StringListLogger : ILogger
    {
        /// <summary>
        /// The single instance of <see cref="StringListLogger"/> which will be returned by <see cref="StringListLoggerProvider"/>, 
        /// and by extension from an <see cref="ILoggerFactory"/> which has called <see cref="StringListLoggerFactoryExtension.AddStringListLogger"/>
        /// 
        /// Use e.g. <code>StringListLogger.Instance.LoggedLines</code> to inspect or assert on logged lines.
        /// </summary>
        /// <remarks>Note that if more than one <see cref="ILoggerFactory"/> uses <see cref="StringListLoggerProvider"/>, 
        /// this Instance will be overwritten by the last factory to create a logger.
        /// </remarks>
        public static StringListLogger Instance;

        /// <summary>
        /// The record of lines logged to this <see cref="StringListLogger"/>
        /// 
        /// If the <see cref="StringListLogger"/> was created by passing in an existing <see cref="List{String}"/>, then this property will be <seealso cref="object.ReferenceEquals"/>() to that list.
        /// </summary>
        public List<string> LoggedLines { get; }

        static readonly string LoglevelPadding = ": ";
        static readonly string MessagePadding = new string(' ', LogLevel.Information.ToString().Length + LoglevelPadding.Length);
        static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;

        [ThreadStatic] static StringBuilder logBuilder;
        Func<string, LogLevel, bool> filter;

        public StringListLogger(List<string> backingList = null, string name=null, Func<string, LogLevel, bool> filter = null, bool includeScopes = true)
        {
            Name = name ?? string.Empty;
            Filter = filter ?? ((category, logLevel) => true);
            IncludeScopes = includeScopes;
            LoggedLines = backingList ?? new List<string>();
        }

        public Func<string, LogLevel, bool> Filter
        {
            protected internal get => filter;
            set => filter = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool IncludeScopes { get; set; }

        public string Name { get; set; }

        public ScopeStack Scopes {get;}= new ScopeStack();

        public class ScopeStack : Stack<Tuple<string, object>>, IDisposable
        {
            public void Dispose(){Pop();}
            public new ScopeStack Push(Tuple<string, object> item){base.Push(item);return this;}
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            string message;
            try
            {
                message = formatter(state, exception);
            }
            catch (Exception e)
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

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message,
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
            foreach(var scope in Scopes)
            {
                var asString = scope.Item2 is Type t ? t.Name : scope.Item2;
                var str = length != builder.Length
                              ? string.Format("=> {0} ", asString)
                              : string.Format("=> {0}",  asString);
                builder.Insert(length, str);
            }

            if (builder.Length <= length)return;
            builder.Insert(length, MessagePadding);
            builder.AppendLine();
        }
    }
}
