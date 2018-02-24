using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TestBase
{
    /// <summary>Factory methods returning a logger which logs to a <see cref="List{String}"/></summary>
    public static class StringListLogger
    {
        public static ILogger WrappedAsMsILogger(IList<string> stringList, string categoryName = "UnitTest")
        {
            return new LoggerFactory().AddSerilog(WrappedAsSerilogger(stringList)).CreateLogger(categoryName);
        }
        public static ILogger<T> WrappedAsMsILogger<T>(IList<string> stringList)
        {
            return new LoggerFactory().AddSerilog(WrappedAsSerilogger(stringList)).CreateLogger<T>();
        }

        public static Logger WrappedAsSerilogger(IList<string> stringList)
        {
            return new LoggerConfiguration().WriteTo.StringList(stringList).CreateLogger();
        }

        /// <summary>
        /// Write log events to the provided <see cref="StringListSink"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="stringList">The string list to write log events to.</param>
        /// <param name="outputTemplate">Message template describing the output format.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink.
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration StringList(
            this LoggerSinkConfiguration sinkConfiguration,
            IList<string> stringList,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = StringListSink.DefaultOutputTemplate,
            IFormatProvider formatProvider = null)
        {
            if (stringList == null) throw new ArgumentNullException("stringList");
            if (outputTemplate == null) throw new ArgumentNullException("outputTemplate");

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var sink = new StringListSink(stringList, formatter);
            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Write log events to the provided <see cref="System.IO.TextWriter"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="stringList">The string list to write log events to.</param>
        /// <param name="formatter">Text formatter used by sink.</param>
        /// /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink.
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration StringList(
            this LoggerSinkConfiguration sinkConfiguration,
            ITextFormatter formatter,
            IList<string> stringList,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (stringList == null) throw new ArgumentNullException("stringList");
            if (formatter == null) throw new ArgumentNullException("formatter");

            var sink = new StringListSink(stringList, formatter);
            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}