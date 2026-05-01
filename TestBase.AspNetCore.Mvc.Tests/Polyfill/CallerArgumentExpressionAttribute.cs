#if !NET5_0_OR_GREATER
    
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>Framework out of date: Use Net50 or higher to access a functioning
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerargumentexpressionattribute"/>
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