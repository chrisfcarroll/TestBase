using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TestBase
{
    public static class JQueryablePropertyValueShoulds
    {
        /// <summary>
        /// <para>Passes if <paramref name="actual"/> has a property named (where named can be a dotted path to a property name) <paramref name="jsonexpression"/> with value <paramref name="expected"/></para>
        /// <para>The property value is extracted using <see cref="JToken.SelectToken(string)"/> and <see cref="JToken.ToObject"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="actual">the value being tested</param>
        /// <param name="jsonexpression">A Json syntax expression naming a property of <paramref name="actual"/> e.g. "length" or "agents.rows[0].telephone"</param>
        /// <param name="expected">the expected value</param>
        /// <returns><paramref name="actual"/> if the assertion passes. Throws otherwise.</returns>
        public static T Property<T,TValue>(this T actual, string jsonexpression, TValue expected,string comment = null, params object[] commentArgs)
        {
            EqualsByValueShoulds.ShouldEqualByValue(actual.ToJQueryable().SelectToken(jsonexpression).ToObject<TValue>(), expected);
            return actual;
        }
        /// <summary>
        /// <para>Returns a property on <paramref name="actual"/> named --where named can also mean a dotted path to a property name--
        /// <paramref name="jsonexpression"/></para>
        /// <para>The property value is extracted using <see cref="JToken.SelectToken(string)"/> and <see cref="JToken.ToObject{T}()"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual">the value being tested</param>
        /// <param name="jsonexpression">A Json syntax expression naming a property of <paramref name="actual"/> e.g. "length" or "agents.rows[0].telephone"</param>
        /// <param name="type">type <see cref="System.Type"/> of the expected value. The found property is converted to this type and then returned.</param>
        /// <returns>The property of <paramref name="actual"/> named by <paramref name="jsonexpression"/></returns>
        public static object Property<T>(this T actual, string jsonexpression, Type type)
        {
            return actual.ToJQueryable().SelectToken(jsonexpression).ToObject(type);
        }

        /// <summary>
        /// <para>Returns a property on <paramref name="actual"/> named �where named can also mean a dotted path to a property name� <paramref name="jsonexpression"/></para>
        /// <para>The property value is extracted using <see cref="JToken.SelectToken(string)"/> and <see cref="JToken.ToObject"/></para>
        /// </summary>
        /// <typeparam name="TValue">The found property is converted to this type and then returned.</typeparam>
        /// <param name="actual">the value being tested</param>
        /// <param name="jsonexpression">A Json syntax expression naming a property of <paramref name="actual"/> e.g. "length" or "agents.rows[0].telephone"</param>
        /// <returns>The property of <paramref name="actual"/> named by <paramref name="jsonexpression"/></returns>
        public static TValue Property<TValue>(this object actual, string jsonexpression)
        {
            return actual.ToJQueryable().SelectToken(jsonexpression).ToObject<TValue>();
        }

        /// <summary>
        /// <para>Returns a property on <paramref name="actual"/> named �where named can also mean a dotted path to a property name� <paramref name="jsonexpression"/></para>
        /// <para>The property value is extracted using <see cref="JToken.SelectToken(string)"/> and <see cref="JToken.ToObject"/></para>
        /// </summary>
        /// <typeparam name="TValue">The found property is converted to <see cref="IEnumerable{TValue}"/> and then returned.</typeparam>
        /// <param name="actual">the value being tested</param>
        /// <param name="jsonexpression">A Json syntax expression naming a property of <paramref name="actual"/> e.g. "length" or "agents.rows[0].telephone"</param>
        /// <returns>The property of <paramref name="actual"/> named by <paramref name="jsonexpression"/>, as a <see cref="IEnumerable{TValue}"/> </returns>
        public static IEnumerable<TValue> PropertyEnumerable<TValue>(this object actual, string jsonexpression)
        {
            return actual.ToJQueryable().SelectToken(jsonexpression).ToObject<IEnumerable<TValue>>();
        }
    }
}