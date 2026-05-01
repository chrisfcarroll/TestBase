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

  /// <summary>Specifies that <see langword="null" /> is allowed as an input even if the corresponding type disallows it.</summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
  public sealed class AllowNullAttribute : Attribute { }

}
#endif