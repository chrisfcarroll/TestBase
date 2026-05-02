using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace TestBase
{
    /// <summary>
    ///     Detects the active test runner by searching loaded
    ///     assemblies, and creates or throws the test-runner specific assertion
    ///     exception so that the runners recognises it as an assertion failure.
    /// <p>Current known test runners are NUnit, xUnit, MSTest</p>
    /// </summary>
    static class KnownTestRunnerAssertions
    {
        /// <summary>
        ///     Wraps <paramref name="assertion"/> in the active test runner's assertion
        ///     exception, preserving it as <see cref="Exception.InnerException"/>.
        ///     Returns <paramref name="assertion"/> unchanged when no framework is detected.
        /// </summary>
        public static Exception From(Exception assertion)
        {
            var type = knownExceptionType.Value;
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
        ///     Wraps <paramref name="assertion"/> in the active test runner's assertion
        ///     exception and throws it. Throws <paramref name="assertion"/> unchanged when
        ///     no framework is detected.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static void Throw(Exception assertion) => throw From(assertion);

        /// <summary>
        ///     Creates the active test runner's inconclusive/skip exception with the given
        ///     <paramref name="message"/>. Returns null when no supported framework is detected.
        /// </summary>
        public static Exception CreateInconclusive(string message)
        {
            var type = knownInconclusiveExceptionType.Value;
            if (type == null) return null;

            try
            {
                var ctor = type.GetConstructor(new[] { typeof(string) });
                if (ctor != null)
                    return (Exception)ctor.Invoke(new object[] { message });
            }
            catch { /* fall through */ }

            return null;
        }

        /// <summary>
        ///     Throws the active test runner's inconclusive/skip exception.
        ///     Falls back to throwing an <see cref="Assertion"/> if no framework is detected.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static void ThrowInconclusive(string message)
        {
            throw CreateInconclusive(message) ?? new Assertion(message).ForActiveTestRunner();
        }

        static readonly Lazy<Type> knownExceptionType = new Lazy<Type>(DetectFrameworkExceptionType,
                                                                       LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<Type> knownInconclusiveExceptionType = new Lazy<Type>(DetectInconclusiveExceptionType,
                                                                           LazyThreadSafetyMode.PublicationOnly);

        static Type DetectFrameworkExceptionType()
        {
            try
            {
                var assemblies = GetLoadedAssemblies();
                if (assemblies.Length == 0) return null;

                return FindTypeInAssemblies("NUnit.Framework.AssertionException", assemblies)
                    ?? FindTypeInAssemblies("Xunit.Sdk.XunitException", assemblies)
                    ?? FindTypeInAssemblies("Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException", assemblies);
            }
            catch { return null; }
        }

        static Type DetectInconclusiveExceptionType()
        {
            try
            {
                var assemblies = GetLoadedAssemblies();
                if (assemblies.Length == 0) return null;

                return FindTypeInAssemblies("NUnit.Framework.InconclusiveException", assemblies)
                    ?? FindTypeInAssemblies("Xunit.SkipException", assemblies)
                    ?? FindTypeInAssemblies("Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException", assemblies);
            }
            catch { return null; }
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
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
            var appDomainType = Type.GetType("System.AppDomain");
            if (appDomainType == null) return new Assembly[0];
            var currentDomainProp = appDomainType.GetProperty("CurrentDomain", BindingFlags.Public | BindingFlags.Static);
            if (currentDomainProp == null) return new Assembly[0];
            var domain = currentDomainProp.GetValue(null);
            var getAssembliesMethod = appDomainType.GetMethod("GetAssemblies", Type.EmptyTypes);
            if (getAssembliesMethod == null) return new Assembly[0];
            return (Assembly[])getAssembliesMethod.Invoke(domain, null);
#else
            return System.Threading.Thread.GetDomain().GetAssemblies();
#endif
        }
    }
}
