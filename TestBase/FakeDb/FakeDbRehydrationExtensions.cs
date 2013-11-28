using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestBase.FakeDb
{
    public static class FakeDbRehydrationExtensions
    {
        public static IEnumerable<string> GetDbRehydratablePropertyNames(this Type type)
        {
            return type.GetProperties()
                       .Where(x => x.CanWrite)
                       .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
                       .Select(x => x.Name);

        }
        public static IEnumerable<PropertyInfo> GetDbRehydratableProperties(this Type type)
        {
            return type.GetProperties()
                       .Where(x => x.CanWrite)
                       .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof (string));

        }
    }
}