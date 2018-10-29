using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestBase.AdoNet
{
    public static class DbRehydrationExtensions
    {
        ///<summary>
        /// A method to simplify the creation of MetaData for return by <see cref="FakeDbCommand"/> so that 
        /// overloads of <see cref="FakeDbCommand.ForExecuteQuery"/> which deduce MetaData by reflection
        /// </summary>
        /// <returns>A list of names of properties of <paramref name="type"/> which can be rehydrated 
        /// from a SQL select from a 'typical' SQL database, namely ValueTypes and Strings, but not complex types.
        /// This method will first consider the result of <see cref="GetWriteableValueTypesAndStringProperties"/>.
        /// If the result is empty, then return <see cref="GetReadableValueTypesAndStringConstructorParameters"/>
        /// </returns>
        public static IEnumerable<string> GetDbRehydratablePropertyNames(this Type type)
        {
            var writeablePrimitives = GetWriteableValueTypesAndStringProperties(type);
            return writeablePrimitives.Any() 
                    ? writeablePrimitives.Select(p => p.Name)
                    : GetReadableValueTypesAndStringConstructorParameters(type).Select(p=>p.Name);
        }

        ///<summary>This method identifies a constructor having parameters which match (by type and case-insensitive name ) the read-only properties 
        /// of an instance of <paramref name="type"/>, and returns the <see cref="ParameterInfo"/>s in question.
        /// </summary>
        /// <returns>A list of names of <strong>readable</strong> properties of <paramref name="type"/> which are 
        /// (1) ValueTypes or string (and not complex types) and (2) can be set in one of <paramref name="type"/>'s constructors.
        /// 
        /// If no suitable constructor is found, returns an empty array.
        /// </returns>
        public static ParameterInfo[] GetReadableValueTypesAndStringConstructorParameters(Type type)
        {
            var readablePrimitives = type.GetProperties()
                                         .Where(x => x.CanRead)
                                         .Where(x => x.PropertyType.GetTypeInfo().IsValueType || x.PropertyType == typeof(string));

            var bestMatchingConstructorParameterList =
                type.GetConstructors()
                    .Select(
                            c => c.GetParameters()
                                  .Where(p => readablePrimitives.Any(
                                                        rp =>  rp.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)
                                                            && rp.PropertyType== p.ParameterType))
                                  .ToArray()
                           )
                    .OrderByDescending(pl => pl.Length)
                    .FirstOrDefault();
                    

            return bestMatchingConstructorParameterList ?? new ParameterInfo[0];

        }

        /// <returns>A list of <strong>writeable</strong> properties of <paramref name="type"/> which can be rehydrated 
        /// from a SQL select: value types and strings but not complex types.</returns>
        public static IEnumerable<PropertyInfo> GetDbRehydratableProperties(this Type type)
        {
            return type.GetProperties()
                       .Where(x => x.CanWrite)
                       .Where(x => x.PropertyType.GetTypeInfo().IsValueType || x.PropertyType == typeof (string));
        }

        public static IEnumerable<PropertyInfo> GetWriteableValueTypesAndStringProperties(this Type type)
        {
            var writeablePrimitives = type.GetProperties()
                                          .Where(x => x.CanWrite)
                                          .Where(x => x.PropertyType.GetTypeInfo().IsValueType || x.PropertyType == typeof(string));
            return writeablePrimitives;
        }
    }
}