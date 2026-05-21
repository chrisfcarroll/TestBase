#if !NET5_0_OR_GREATER
    
// ReSharper disable CheckNamespace

namespace System.Runtime.CompilerServices
{
    /// <summary>Polyfill for
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerargumentexpressionattribute"/>
    /// for runtimes older than net5.0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}

#endif