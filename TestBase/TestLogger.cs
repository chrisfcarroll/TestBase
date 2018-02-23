using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TestBase
{
    /// <summary>
    /// TestLogger is based on Enterprise Library Logger. 
    /// 
    /// If your application defines a logger interface which is similar to the EntLib logger, then an adapter 
    /// might be as simply as public class MyLogger : TestLogger, IMyAppsLoggerInterface {}
    /// 
    /// Verify that your UnitUnderTest logged correctly after an action by inspecting <see cref="TestLogger.LoggedEntries"/> 
    /// </summary>
    public class TestLogger
    {
        private readonly Guid logGuid = Guid.NewGuid();

        /// <summary>
        /// 
        /// </summary>
        public List<TestLogEntry> LoggedEntries = new List<TestLogEntry>();

        public TraceEventType OnlyLogLevelsAtAndBelow = TraceEventType.Transfer;

        protected const TraceEventType DebugLevel = TraceEventType.Verbose;
        protected const TraceEventType InfoLevel = TraceEventType.Information;
        protected const TraceEventType WarnLevel = TraceEventType.Warning;
        protected const TraceEventType ErrorLevel = TraceEventType.Error;
        protected const TraceEventType FatalLevel = TraceEventType.Critical;

        private void Log(TraceEventType level, string format, params object[] args)
        {
            if (!ShouldLogAtLevel(level)) return;

            WriteLog(level, string.Format(format, args));
        }

        private void Log(TraceEventType level, string message, Exception exception)
        {
            if (!ShouldLogAtLevel(level)) return;

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            new XmlExceptionFormatter(writer, exception, logGuid).Format();

            WriteLog(level, string.Format("{0}{1}{2}", message, Environment.NewLine, sb));
        }

        private void WriteLog(TraceEventType level, string message)
        {
            var entry = CreateLogEntry(level);
            entry.Message = message;
            LoggedEntries.Add(entry);
        }

        private bool ShouldLogAtLevel(TraceEventType level)
        {
            return  level <= OnlyLogLevelsAtAndBelow;
        }

        private static TestLogEntry CreateLogEntry(TraceEventType level)
        {
            var entry = new TestLogEntry
            {
                Severity = level,
                Priority = (int)level,
                Title = "TwentyTwenty.Airtime.Web"
            };
            entry.Categories.Add(level.ToString());

            return entry;
        }

        public void Fatal(string message, Exception exception)
        {
            Log(FatalLevel, message, exception);
        }

        public void Fatal(string message)
        {
            Log(FatalLevel, message);
        }

        public void Fatal(string format, params object[] args)
        {
            Log(FatalLevel, format, args);
        }

        public void Error(string message, Exception exception)
        {
            Log(ErrorLevel, message, exception);
        }

        public void Error(string message)
        {
            Log(ErrorLevel, message);
        }

        public void Error(string format, params object[] args)
        {
            Log(ErrorLevel, format, args);
        }

        public void Warn(string message, Exception exception)
        {
            Log(WarnLevel, message, exception);
        }

        public void Warn(string message)
        {
            Log(WarnLevel, message);
        }

        public void Warn(string format, params object[] args)
        {
            Log(WarnLevel, format, args);
        }

        public void Info(string message, Exception exception)
        {
            Log(InfoLevel, message, exception);
        }

        public void Info(string message)
        {
            Log(InfoLevel, message);
        }

        public void Info(string format, params object[] args)
        {
            Log(InfoLevel, format, args);
        }

        public void Debug(string message, Exception exception)
        {
            Log(DebugLevel, message, exception);
        }

        public void Debug(string message)
        {
            Log(DebugLevel, message);
        }

        public void Debug(string format, params object[] args)
        {
            Log(DebugLevel, format, args);
        }
    }

    public class TestLogEntry
    {
        public string Message { get; set; }
        public ICollection<string> Categories = new List<string>();
        public int Priority { get; set; }
        public int EventId { get; set; }
        public TraceEventType Severity { get; set; }
        public string Title { get; set; }
        public DateTime TimeStamp { get; set; }
        public string MachineName { get; set; }
        public string AppDomainName { get; set; }
        public string ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string ManagedThreadName { get; set; }
        public string Win32ThreadId { get; set; }
        public IDictionary<string, object> ExtendedProperties = new Dictionary<string, object>();
        public Guid ActivityId { get; set; }
        public Guid? RelatedActivityId { get; set; }
    }
}
