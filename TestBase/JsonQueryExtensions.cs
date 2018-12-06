using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestBase
{
    public static class JsonQueryExtensions
    {
        /// <summary>
        ///     Returns a <see cref="JObject" /> which can be queried.
        ///     See https://www.newtonsoft.com/json/help/html/SelectToken.htm#SelectTokenJSONPath
        ///     for examples with Javascript,JPath &amp; Linq
        ///     <list type="bullet">
        ///         <item>Indexers e.g. (int)jobject["rows"][0]["id"]</item>
        ///         <item>SelectToken() with javascript syntax, e.g. (int)jobject["rows[0].id"]</item>
        ///         <item>SelectTokens() with JPath syntax, e.g. (int)jobject["$..rows[?(@.Price >= 50)].id"]</item>
        ///         <item>Linq, e.g. jobject["rows"].Sum(r => (decimal)r.SelectToken("Price"))</item>
        ///     </list>
        /// </summary>
        /// <returns>A Queryable <see cref="JObject" />. </returns>
        public static JObject ToJQueryable<T>(this T value)
        {
            return JObject.Parse(JsonConvert.SerializeObject(value));
        }

        /// <summary>
        ///     Returns a <see cref="JObject" /> which can be queried.
        ///     See https://www.newtonsoft.com/json/help/html/SelectToken.htm#SelectTokenJSONPath
        ///     for examples with Javascript,JPath &amp; Linq
        ///     <list type="bullet">
        ///         <item>Indexers e.g. (int)jobject["rows"][0]["id"]</item>
        ///         <item>SelectToken() with javascript syntax, e.g. (int)jobject["rows[0].id"]</item>
        ///         <item>SelectTokens() with JPath syntax, e.g. (int)jobject["$..rows[?(@.Price >= 50)].id"]</item>
        ///         <item>Linq, e.g. jobject["rows"].Sum(r => (decimal)r.SelectToken("Price"))</item>
        ///     </list>
        /// </summary>
        /// <returns>A Queryable <see cref="JObject" />. </returns>
        public static JObject ToQueryable<T>(this T value, Formatting formatting)
        {
            return JObject.Parse(JsonConvert.SerializeObject(value, formatting));
        }

        /// <summary>
        ///     Returns a <see cref="JObject" /> which can be queried.
        ///     See https://www.newtonsoft.com/json/help/html/SelectToken.htm#SelectTokenJSONPath
        ///     for examples with Javascript,JPath &amp; Linq
        ///     <list type="bullet">
        ///         <item>Indexers e.g. (int)jobject["rows"][0]["id"]</item>
        ///         <item>SelectToken() with javascript syntax, e.g. (int)jobject["rows[0].id"]</item>
        ///         <item>SelectTokens() with JPath syntax, e.g. (int)jobject["$..rows[?(@.Price >= 50)].id"]</item>
        ///         <item>Linq, e.g. jobject["rows"].Sum(r => (decimal)r.SelectToken("Price"))</item>
        ///     </list>
        /// </summary>
        /// <returns>A Queryable <see cref="JObject" />. </returns>
        public static JObject ToQueryable<T>(this T value, Formatting formatting, JsonSerializerSettings settings)
        {
            return JObject.Parse(JsonConvert.SerializeObject(value, formatting, settings));
        }

        public static string ToJSon<T>(this T value) { return JsonConvert.SerializeObject(value); }

        public static string ToJSon<T>(this T value, Formatting formatting)
        {
            return JsonConvert.SerializeObject(value, formatting);
        }

        public static string ToJSon<T>(this T value, Formatting formatting, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, formatting, settings);
        }

        public static T FromJSon<T>(this string json) { return JsonConvert.DeserializeObject<T>(json); }

        public static T FromJSon<T>(this string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static T ShouldBeAssignableTo<T>(this JToken actual, string comment = null, params object[] args)
        where T : class
        {
            return actual.ToObject<T>().ShouldBeAssignableTo<T>(comment, args);
        }

        public static T ShouldBeCastableTo<T>(this JToken actual, string comment = null, params object[] args)
        {
            return actual.ToObject<T>().ShouldBeCastableTo<T>(comment, args);
        }

        public static T As<T>(this JToken actual, string comment = null, params object[] args)
        {
            return actual.ToObject<T>().ShouldBeCastableTo<T>(comment, args);
        }

        public static IEnumerable<T> AsEnumerable<T>(this JToken actual) { return actual.Select(a => a.ToObject<T>()); }
    }
}
