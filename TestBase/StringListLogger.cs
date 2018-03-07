using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TestBase
{
    public static class StringListLoggerFactoryExtension
    {
        /// <summary>
        /// Ensures that when a <see cref="StringListLogger"/> is requested for name <paramref name="name"/>, 
        /// the returned Logger will write to <paramref name="backingList"/>
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="name"></param>
        /// <param name="backingList"></param>
        /// <param name="includeScopes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory, string name, List<string> backingList, bool includeScopes = true, Func<string, LogLevel, bool> filter = null)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            filter = filter ?? ((s,t)=>true);
            var soleInstance= new StringListLogger(backingList, name, filter, includeScopes);
            factory.AddProvider(new StringListLoggerSingleInstanceProvider(soleInstance));
            return factory;
        }

        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory, StringListLogger soleInstance)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            factory.AddProvider(new StringListLoggerSingleInstanceProvider(soleInstance));
            return factory;
        }
        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory)
        {
            return AddStringListLogger(factory, new StringListLogger());
        }
    }

    public class StringListLoggerSingleInstanceProvider : ILoggerProvider
    {
        Stack<string> names= new Stack<string>();

        public StringListLoggerSingleInstanceProvider(StringListLogger instance){StringListLogger.Instance =   instance; }

        public StringListLoggerSingleInstanceProvider(): this(new StringListLogger()){}

        public void Dispose(){ StringListLogger.Instance.Name=names.Pop();} 

        public ILogger CreateLogger(string categoryName)
        {
            names.Push( StringListLogger.Instance.Name = categoryName) ;
            return StringListLogger.Instance;
        }
    }

    public class StringListLogger : ILogger
    {
        public static StringListLogger Instance;

        static readonly string LoglevelPadding = ": ";

        static readonly string MessagePadding = new string(' ', LogLevel.Information.ToString().Length + LoglevelPadding.Length);

        static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;
        [ThreadStatic] static StringBuilder logBuilder;
        Func<string, LogLevel, bool> filter;
        static readonly JsonSerializerSettings ErrorSwallowingJsonSerializerSettings = new JsonSerializerSettings{Error = (o, e) => { }, ReferenceLoopHandling = ReferenceLoopHandling.Ignore};

        public StringListLogger(List<string> backingList = null, string name=null, Func<string, LogLevel, bool> filter = null, bool includeScopes = true)
        {
            Name = name ?? string.Empty;
            Filter = filter ?? ((category, logLevel) => true);
            IncludeScopes = includeScopes;
            LoggedLines = backingList ?? new List<string>();
        }

        public List<string> LoggedLines { get; }

        public Func<string, LogLevel, bool> Filter
        {
            protected internal get => filter;
            set => filter = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool IncludeScopes { get; set; }

        public string Name { get; set; }

        ScopeStack Scopes {get;}= new ScopeStack();

        public class ScopeStack : Stack<(string, object)>, IDisposable
        {
            public void Dispose(){Pop();}
            public new ScopeStack Push((string,object) item){base.Push(item);return this;}
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
            Scopes.Push((Name, state));
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


    //public class StringListLoggerByNameProvider : ILoggerProvider
    //{

    //    internal static readonly Func<string, LogLevel, bool> FalseFilter = (cat, level) => false;
    //    internal static readonly Func<string, LogLevel, bool> TrueFilter = (cat, level) => true;

    //    public static Func<string, LogLevel, bool> DefaultFilter { get; set; } = TrueFilter;
    //    public static bool DefaultIncludeScopes { get; set; } = true;

    //    public static StringListLoggerByNameProvider Instance { get; } = new StringListLoggerByNameProvider();


    //    public ConcurrentDictionary<string, StringListLogger> Loggers { get; } = new ConcurrentDictionary<string, StringListLogger>();

    //    public ILogger CreateLogger(string name){ return Loggers.GetOrAdd(name, n => CreateLoggerImplementation(n, TrueFilter, true)); }

    //    public void Dispose() { }

    //    public StringListLogger CreateLoggerImplementation(string name, Func<string, LogLevel, bool> filter, bool includeScopes)
    //    {
    //        return new StringListLogger(new List<string>(), name, filter ?? FalseFilter, includeScopes);
    //    }

    //}
}
