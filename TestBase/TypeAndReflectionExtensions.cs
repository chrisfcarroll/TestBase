using System;
using System.Linq;
using System.Reflection;

namespace TestBase
{
    public static class TypeAndReflectionExtensions
    {
        public static object GetPropertyValue<T>(this PropertyInfo propertyInfo, T obj, string propertyName)
        {
            var nestedPropertyInfo = propertyInfo;
            object nestedObject = obj;
            var nestedPropertyName = propertyName;
            object nullOrDefault = default(T);

            while (!Equals(nestedObject, nullOrDefault) && nestedPropertyName.Contains("."))
            {
                var nameBeforeDot = nestedPropertyName.Substring(0, propertyName.IndexOf('.'));
                var beforeDot = GetPropertyInfo(nestedObject.GetType(), nameBeforeDot);
                var typeBeforeDot = beforeDot.PropertyType;
                var nameAfterDot = nestedPropertyName.Substring(1 + nestedPropertyName.IndexOf('.'));
                //var afterDot = GetPropertyInfo(nameAfterDot, typeBeforeDot);
                //nestedPropertyInfo=afterDot;
                nestedObject = beforeDot.GetValue(nestedObject, null);
                nestedPropertyName = nameAfterDot;
                nullOrDefault = typeBeforeDot.IsValueType ? Activator.CreateInstance((Type) typeBeforeDot) : null;
            }

            return Equals(nestedObject, nullOrDefault)
                ? nestedPropertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(nestedPropertyInfo.PropertyType) : null
                : nestedPropertyInfo.GetValue(nestedObject, null);
        }

        public static PropertyInfo GetPropertyInfo(this Type objectType, string propertyName)
        {
            if (propertyName.Contains("."))
            {
                var nameBeforeDot = propertyName.Substring(0, propertyName.IndexOf('.'));
                var beforeDot = GetPropertyInfo(objectType, nameBeforeDot);
                EnsurePropertyOrThrow(objectType, beforeDot, nameBeforeDot);
                var typeBeforeDot = beforeDot.PropertyType;
                var nameAfterDot = propertyName.Substring(1 + propertyName.IndexOf('.'));
                var afterDot = GetPropertyInfo(typeBeforeDot, nameAfterDot);
                return afterDot;
            }
            else
            {
                var propertyInfo = objectType.GetProperty(propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public |
                    BindingFlags.Instance);
                return propertyInfo;

            }
        }

        public static void EnsurePropertyOrThrow<T>(this PropertyInfo propertyInfo, string propertyName)
        {
            EnsurePropertyOrThrow(typeof(T), propertyInfo, propertyName);
        }

        public static void EnsurePropertyOrThrow(this Type type, PropertyInfo propertyInfo, string propertyName)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentException(
                    String.Format("Didn't find a public property \"{1}\" of type {0} which has properties ({2}).",
                        type, propertyName, String.Join(", ", type.GetProperties().Cast<PropertyInfo>())),
                    "propertyName");
            }
        }
    }
}
