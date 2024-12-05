#if !NET6_0_OR_GREATER
    
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>Framework out of date: Use Net50 or higher to access a functioning
    /// <see cref="System.Runtime.CompilerServices"/>.CallerArgumentExpressionAttribute
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