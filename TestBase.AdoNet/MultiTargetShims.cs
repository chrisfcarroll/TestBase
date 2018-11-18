using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestBase.AdoNet
{
    
#if NETSTANDARD
#else
    /// <summary>
    /// Extension methods to ease net40«--»netstandard code sharing.
    /// Backfills methods in NetStandard but not in Net40
    /// </summary>
    public static class MultiTargetShims
    {
        /// <summary>Replace FastCompiler with <see cref="Expression{TDelegate}.Compile()"/></summary>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>T</returns>
        public static T CompileFast<T>(this Expression<T> expression) => expression.Compile();

        /// <summary>Shim for <c>GetTypeInfo()</c>, returns the <see cref="Type"/> instead</summary>
        /// <param name="type"></param>
        /// <returns><paramref name="type"/></returns>
        public static Type GetTypeInfo(this Type type) => type;

        /// <summary>Shim for <c>GetCustomAttributes&lt;T%gt;</c>
        /// using <see cref="MemberInfo.GetCustomAttributes(Type,bool)"/> </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns>
        /// <c><![CDATA[@this.GetCustomAttributes(typeof(T),true).Cast<T>().FirstOrDefault()]]></c>
        /// </returns>
        public static T GetCustomAttribute<T>(this PropertyInfo @this) where T : Attribute
        {
            return @this.GetCustomAttributes(typeof(T),true).Cast<T>().FirstOrDefault();
        }
    }
#endif
}
