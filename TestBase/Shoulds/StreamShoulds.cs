using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    static internal class StreamShoulds
    {
        public static Stream ShouldContain(this Stream @this, Stream expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(@this, new StreamContainsConstraint(expectedValue), message??"Stream did not include expected content.", args);
            return @this;
        }

        public static Stream ShouldContain(this Stream @this, byte[] expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(@this, new StreamContainsConstraint(expectedValue), message??"Stream did not include expected content.", args);
            return @this;
        }
    }
}