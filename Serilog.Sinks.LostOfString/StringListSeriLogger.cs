using System;
using System.Collections.Generic;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.ListOfString
{
    /// <summary>
    ///     Factory methods returning a logger which logs to a <see cref="List{T}" />
    ///     Equivalent to <code>new LoggerConfiguration().WriteTo.StringList(stringList).CreateLogger();</code>
    /// </summary>
    public static class StringListSeriLogger
    {
        public static Logger AsSeriLogger(this IList<string> stringList)
        {
            return new LoggerConfiguration().WriteTo.StringList(stringList).CreateLogger();
        }

        /// <summary>
        ///     Write log events to the provided <see cref="ListOfStringSink" />.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="stringList">The string list to write log events to.</param>
        /// <param name="outputTemplate">Message template describing the output format.</param>
        /// <param name="restrictedToMinimumLevel">
        ///     The minimum level for
        ///     events passed through the sink.
        ///     <returns>Configuration object allowing method chaining.</returns>
        ///     <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        ///     <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration StringList(
            this LoggerSinkConfiguration sinkConfiguration,
            IList<string>                stringList,
            LogEventLevel                restrictedToMinimumLevel = LevelAlias.Minimum,
            string                       outputTemplate           = ListOfStringSink.DefaultOutputTemplate,
            IFormatProvider              formatProvider           = null)
        {
            if (stringList     == null) throw new ArgumentNullException("stringList");
            if (outputTemplate == null) throw new ArgumentNullException("outputTemplate");

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var sink      = new ListOfStringSink(stringList, formatter);
            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        ///     Write log events to the provided <see cref="System.IO.TextWriter" />.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="stringList">The string list to write log events to.</param>
        /// <param name="formatter">Text formatter used by sink.</param>
        /// ///
        /// <param name="restrictedToMinimumLevel">
        ///     The minimum level for
        ///     events passed through the sink.
        ///     <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration StringList(
            this LoggerSinkConfiguration sinkConfiguration,
            ITextFormatter               formatter,
            IList<string>                stringList,
            LogEventLevel                restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (stringList == null) throw new ArgumentNullException("stringList");
            if (formatter  == null) throw new ArgumentNullException("formatter");

            var sink = new ListOfStringSink(stringList, formatter);
            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
