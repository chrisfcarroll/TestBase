using System;

namespace TestBase.Attributes
{
    /// <summary>
    /// This is an informational marker tag to persuade you to keep fitnesse tests up to date 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class FitnesseUseExpectedAttribute : Attribute {}
}
