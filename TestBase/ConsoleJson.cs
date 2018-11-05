using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;

namespace TestBase
{
    /// <summary>A wrapper for <see cref="Console.Out"/> which writes objects as Json.</summary>
    public static class ConsoleJson
    {
        /// <summary>
        /// Write <paramref name="objects"/> to the console as Json strings.
        /// </summary>
        public static void WriteLine(params object[] objects) => WriteLine((IEnumerable) objects);

        /// <summary>
        /// Write <paramref name="objects"/> to the console as Json strings.
        /// </summary>
        public static void WriteLine(IEnumerable objects)
        {
            foreach (var item in objects)
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(item));
            }
        }

        /// <summary>Write <paramref name="objects"/> to the console as Json strings.</summary>
        public static T[] WriteJson<T>(this TextWriter console,  params T[] objects)
        {
            foreach (var item in objects)
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(item));
            }
            return objects;
        }

        /// <summary>Write <paramref name="objects"/> to the console as Json strings.</summary>
        public static IEnumerable WriteJson(IEnumerable objects)
        {
            foreach (var item in objects)
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(item));
            }
            return objects;
        }
    }
}