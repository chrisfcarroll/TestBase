using System;
using NUnit.Framework;

namespace TestBase.Attributes
{
    /// <summary>
    /// A Test is not a test if it is in fact a test of the environment or some other dependency rather than an actual test of our code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class EnvironmentalDependencyCheckAttribute : TestAttribute
    {
        public EnvironmentalDependencyCheckAttribute() {}

        public EnvironmentalDependencyCheckAttribute(string description)
        {
            Description = description;
        }
    }
}