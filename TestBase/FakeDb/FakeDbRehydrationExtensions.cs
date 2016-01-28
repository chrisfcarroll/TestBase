using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestBase.FakeDb
{
    public static class FakeDbRehydrationExtensions
    {
        /// <returns>A list of property names of <paramref name="type"/> which can be turned into SQL parameters: value types and strings but not complex types.</returns>
        public static IEnumerable<string> GetDbParameterisablePropertyNames(this Type type)
        {
            return type.GetProperties()
                       .Where(x => x.CanRead)
                       .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
                       .Select(x => x.Name);
        }

        /// <returns>A list of names of <strong>writeable</strong> properties of <paramref name="type"/> which can be rehydrated 
        /// from a SQL select: value types and strings but not complex types.</returns>
        public static IEnumerable<string> GetDbRehydratablePropertyNames(this Type type)
        {
            return type.GetProperties()
                       .Where(x => x.CanWrite)
                       .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
                       .Select(x => x.Name);
        }

        /// <returns>A list of <strong>writeable</strong> properties of <paramref name="type"/> which can be rehydrated 
        /// from a SQL select: value types and strings but not complex types.</returns>
        public static IEnumerable<PropertyInfo> GetDbRehydratableProperties(this Type type)
        {
            return type.GetProperties()
                       .Where(x => x.CanWrite)
                       .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof (string));
        }
    }
}