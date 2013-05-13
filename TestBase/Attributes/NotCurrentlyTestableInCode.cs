using System;
using NUnit.Framework;

namespace TestBase.Attributes
{
    /// <summary>
    /// Informational tag for methods we wish we could test in code but currently can't.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class NotCurrentlyTestableInCode : TestAttribute
    {
        public NotCurrentlyTestableInCode() { }

        public NotCurrentlyTestableInCode(string description)
        {
            Description = description;
        }
    }
}