using System;
using Newtonsoft.Json;

namespace TestBase
{
    /// <summary>An instance of <see cref="JsonSerializerSettings"/> which will make a best effort to complete serialization,
    /// using NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore,
    /// ReferenceLoopHandling = ReferenceLoopHandling.Ignore.
    /// 
    /// Intended for logging.
    /// </summary>
    public static class BestEffortJsonSerializerSettings
    {
        /// <summary><c>new JsonSerializerSettings
        /// {
        /// NullValueHandling = NullValueHandling.Ignore, 
        /// MissingMemberHandling = MissingMemberHandling.Ignore, 
        /// ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        /// Error = (sender, args) =&gt; { }
        /// }<c/> 
        /// </summary>
        public static readonly JsonSerializerSettings Serializer =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore, 
                MissingMemberHandling = MissingMemberHandling.Ignore, 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, args) => { }
            };

        static BestEffortJsonSerializerSettings()
        {
            Serializer.Converters.Add(new DBNullConverter());
        }

        /// <summary>Converts <see cref="DBNull" /> to and from its name string value.</summary>
        public class DBNullConverter : JsonConverter
        {
            /// <summary>Writes the JSON representation of the object.</summary>
            /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteNull();
            }

            /// <summary>Reads the JSON representation of the object.</summary>
            /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return value;
            }

            /// <summary>
            /// Determines whether this instance can convert the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns>
            /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
            /// </returns>
            public override bool CanConvert(Type objectType)
            {
                return objectType.FullName == "System.DBNull";
            }

            /// <summary>
            /// Polyfill for the fact that NetStandard 1.3 is missing System.DBNull, although NetFx has it since v1.1
            /// </summary>
            public static readonly object value = TestBase.DBNull.Value;
        }
    }
}