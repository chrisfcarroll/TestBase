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
    [AttributeUsage(AttributeTargets.Property
                    | AttributeTargets.Field
                    | AttributeTargets.Parameter,
                    Inherited = false)]
    public sealed class AllowNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property
                    | AttributeTargets.Parameter
                    | AttributeTargets.ReturnValue,
                    AllowMultiple = true,
                    Inherited = false)]
    public sealed class NotNullIfNotNullAttribute : Attribute
    {
        /// <summary>Initializes the attribute with the associated parameter name.</summary>
        /// <param name="parameterName">The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.</param>
        public NotNullIfNotNullAttribute(string parameterName) { }

        /// <summary>Gets the associated parameter name.</summary>
        /// <returns>The associated parameter name. The output will be non-null if the argument to the parameter specified is non-null.</returns>
        public string ParameterName { get; }
    }
}
#endif