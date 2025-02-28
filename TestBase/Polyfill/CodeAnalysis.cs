#if !(NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)
    
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>Polyfill for 
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.notnullattribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field 
                    | AttributeTargets.Parameter 
                    | AttributeTargets.Property 
                    | AttributeTargets.ReturnValue)]
    internal sealed class NotNullAttribute : Attribute { }
}

#endif