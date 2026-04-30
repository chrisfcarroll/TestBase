using System;
using System.Reflection;
using System.Threading;

namespace TestBase
{
    /// <summary>
    ///     Detects the active test framework (NUnit, xUnit, or MSTest) by searching loaded
    ///     assemblies, and creates or throws the framework-specific assertion exception so
    ///     that test runners recognise it as an assertion failure.
    /// </summary>
    public static class KnownTestRunnerAssertions
    {
        /// <summary>
        ///     Wraps <paramref name="assertion"/> in the active test framework's assertion
        ///     exception, preserving it as <see cref="Exception.InnerException"/>.
        ///     Returns <paramref name="assertion"/> unchanged when no framework is detected.
        /// </summary>
        public static Exception Create(Exception assertion)
        {
            var type = _frameworkExceptionType.Value;
            if (type == null) return assertion;

            try
            {
                var ctor = type.GetConstructor(new[] { typeof(string), typeof(Exception) });
                if (ctor != null)
                    return (Exception)ctor.Invoke(new object[] { assertion.Message, assertion });

                ctor = type.GetConstructor(new[] { typeof(string) });
                if (ctor != null)
                    return (Exception)ctor.Invoke(new object[] { assertion.Message });
            }
            catch { /* fall through */ }

            return assertion;
        }

        /// <summary>
        ///     Wraps <paramref name="assertion"/> in the active test framework's assertion
        ///     exception and throws it. Throws <paramref name="assertion"/> unchanged when
        ///     no framework is detected.
        /// </summary>
        public static void Throw(Exception assertion)
        {
            throw Create(assertion);
        }

        static readonly Lazy<Type> _frameworkExceptionType = new Lazy<Type>(DetectFrameworkExceptionType, LazyThreadSafetyMode.PublicationOnly);

        static Type DetectFrameworkExceptionType()
        {
            var assemblies = GetLoadedAssemblies();
            if (assemblies.Length == 0) return null;

            return FindTypeInAssemblies("NUnit.Framework.AssertionException", assemblies)
                ?? FindTypeInAssemblies("Xunit.Sdk.XunitException", assemblies)
                ?? FindTypeInAssemblies("Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException", assemblies);
        }

        static Type FindTypeInAssemblies(string fullTypeName, Assembly[] assemblies)
        {
            foreach (var asm in assemblies)
            {
                try
                {
                    var type = asm.GetType(fullTypeName);
                    if (type != null) return type;
                }
                catch { /* assembly may not be loadable */ }
            }
            return null;
        }

        static Assembly[] GetLoadedAssemblies()
        {
            try
            {
                var appDomainType = Type.GetType("System.AppDomain");
                if (appDomainType == null) return new Assembly[0];
                var currentDomainProp = appDomainType.GetProperty("CurrentDomain", BindingFlags.Public | BindingFlags.Static);
                if (currentDomainProp == null) return new Assembly[0];
                var domain = currentDomainProp.GetValue(null);
                var getAssembliesMethod = appDomainType.GetMethod("GetAssemblies", Type.EmptyTypes);
                if (getAssembliesMethod == null) return new Assembly[0];
                return (Assembly[])getAssembliesMethod.Invoke(domain, null);
            }
            catch { return new Assembly[0]; }
        }
    }
}
