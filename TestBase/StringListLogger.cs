using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TestBase
{
    public static class StringListLoggerFactoryExtension
    {
        public static ILoggerFactory AddStringListLogger(this ILoggerFactory factory, List<string> backingList = null, string name = "TestBase")
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            factory.AddProvider(new StringListLoggerProvider(backingList, name));
            return factory;
        }
    }

    public class StringListLogger : ILogger
    {
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

        public string Name { get; }

        ScopeStack Scopes {get;}= new ScopeStack();

        public class ScopeStack : Stack<(string, object)>, IDisposable
        {
            public void Dispose(){Pop();}
            public new ScopeStack Push((string,object) item){base.Push(item);return this;}
        }

        void SwallowError(EventArgs e){}

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            string message;
            try
            {
                try
                {
                    message = formatter(state, exception);
                }
                catch (FormatException)
                {
                    /*
                     * https://github.com/aspnet/Logging/issues/767
                     */
                    var stateFields = state.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var values = stateFields.Select(f => f.GetValue(state)).ToArray();
                    var asJson = JsonConvert.SerializeObject(values, ErrorSwallowingJsonSerializerSettings);
                    message = $"{asJson} {exception}";
                }
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


    [ProviderAlias("StringList")]
    public class StringListLoggerProvider : ILoggerProvider, IDisposable
    {
        static readonly Func<string, LogLevel, bool> FalseFilter = (cat, level) => false;
        static readonly Func<string, LogLevel, bool> TrueFilter = (cat, level) => true;
        readonly Func<string, LogLevel, bool> filter;
        readonly bool includeScopes;

        public StringListLoggerProvider()
        {
            filter = TrueFilter;
            includeScopes = true;
        }

        public StringListLoggerProvider(List<String> backingList = null, string name = null,
                                        Func<string, LogLevel, bool> filter = null, bool includeScopes = true)
        {
            this.filter = filter ?? TrueFilter;
            this.includeScopes = includeScopes;
            if (name        != null && backingList == null) CreateLogger(name);
            if (backingList != null)
                Loggers.GetOrAdd(name ?? String.Empty,
                                 s => new StringListLogger(backingList, s, this.filter, this.includeScopes));
        }

        public StringListLoggerProvider(List<string> logger) { Loggers.TryAdd("", new StringListLogger(logger)); }

        public ConcurrentDictionary<string, StringListLogger> Loggers { get; } =
            new ConcurrentDictionary<string, StringListLogger>();

        public ILogger CreateLogger(string name)
        {
            return Loggers.GetOrAdd(name ?? String.Empty, CreateLoggerImplementation);
        }

        public void Dispose() { }

        StringListLogger CreateLoggerImplementation(string name)
        {
            return new StringListLogger(new List<string>(), name, GetFilter(name), includeScopes);
        }

        Func<string, LogLevel, bool> GetFilter(string name) { return filter ?? FalseFilter; }
    }
}
