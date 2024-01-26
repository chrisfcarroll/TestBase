using System;

namespace TestBase
{
#if !NET5_0_OR_GREATER
    
    /// <summary>Framework out of date: Use Net50 or higher to access a functioning
    /// <see cref="System.Runtime.CompilerServices"/>.CallerArgumentExpressionAttribute
    /// </summary>
    class CallerArgumentExpressionAttribute : Attribute
    {
        readonly string parameterName;
        public CallerArgumentExpressionAttribute(string expression) => this.parameterName = expression;

        public override string ToString() => parameterName;
    }

#endif    
   
}