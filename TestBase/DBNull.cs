using System;
using System.Reflection;

namespace TestBase
{
    /// <summary>
    /// Polyfill for the fact that NetStandard 1.3 is missing System.DBNull, although NetFx has it since v1.1
    /// </summary>
    class DBNull
    {
        /// <summary>
        /// Polyfill for System.DBNull.Value, because NetStandard 1.3 is missing System.DBNull, although NetFx has it since v1.1
        /// </summary>
        public static readonly object Value = Type.GetType("System.DBNull")
            ?.GetField("Value", BindingFlags.Public | BindingFlags.Static)
            ?.GetValue(null);
    }
}