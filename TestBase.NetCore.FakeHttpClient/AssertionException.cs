using System.Reflection;

namespace TestBase;

/// <summary>Thrown when a FakeHttpClient assertion fails.</summary>
public class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }

    /// <summary>
    ///     Returns this assertion wrapped in the active test framework's assertion exception
    ///     (NUnit, xUnit, or MSTest) so that test runners recognise it as an assertion failure.
    ///     Returns <c>this</c> unchanged when no supported test framework is detected.
    /// </summary>
    public Exception ForActiveTestRunner()
    {
        var type = _frameworkExceptionType.Value;
        if (type == null) return this;

        try
        {
            var ctor = type.GetConstructor(new[] { typeof(string), typeof(Exception) });
            if (ctor != null)
                return (Exception)ctor.Invoke(new object[] { Message, this });

            ctor = type.GetConstructor(new[] { typeof(string) });
            if (ctor != null)
                return (Exception)ctor.Invoke(new object[] { Message });
        }
        catch { /* fall through */ }

        return this;
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
