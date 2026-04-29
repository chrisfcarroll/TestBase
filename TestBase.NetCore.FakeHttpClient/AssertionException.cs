using System.Reflection;

namespace TestBase;

/// <summary>Thrown when a FakeHttpClient assertion fails.</summary>
public class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }

    /// <summary>
    ///     Wraps an assertion exception in the appropriate test-framework-specific exception
    ///     (NUnit, xUnit, or MSTest) so that test runners recognise it as an assertion failure.
    ///     If no supported test framework is detected, returns the original exception unchanged.
    /// </summary>
    public static Exception CreateFrameworkException(Exception assertion)
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

    static readonly Lazy<Type?> _frameworkExceptionType = new(DetectFrameworkExceptionType, System.Threading.LazyThreadSafetyMode.PublicationOnly);

    static Type? DetectFrameworkExceptionType()
    {
        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return FindTypeInAssemblies("NUnit.Framework.AssertionException", assemblies)
                ?? FindTypeInAssemblies("Xunit.Sdk.XunitException", assemblies)
                ?? FindTypeInAssemblies("Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException", assemblies);
        }
        catch { return null; }
    }

    static Type? FindTypeInAssemblies(string fullTypeName, Assembly[] assemblies)
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
}
