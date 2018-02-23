using System;
using System.IO;

namespace TestBase.Tests
{
    public static class LoggingExtensions
    {
        public static T LogIf<T>(this T @this, TextWriter console=null)
        {
            console = console ?? Console.Out;
            if (Properties.Settings.Default.Verbose)
            {
                console.WriteLine(@this);
            }
            return @this;
        }
    }
}
