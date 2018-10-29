using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    static internal class StreamShoulds
    {
        public static Stream ShouldContain(this Stream @this, Stream expectedValue, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, new StreamContainsConstraint(expectedValue), message??"Stream did not include expected content.", args);
            return @this;
        }

        public static Stream ShouldContain(this Stream @this, byte[] expectedValue, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, new StreamContainsConstraint(expectedValue), message??"Stream did not include expected content.", args);
            return @this;
        }

        public static Stream ShouldEqualByValue(this Stream @this, Stream expectedValue, [Optional] string message)
        {
            @this.Position = 0;
            expectedValue.Position = 0;
            var left = new BufferedStream(@this);
            var right = new BufferedStream(expectedValue);
            byte[] bufLeft = new byte[32];
            byte[] bufRight = new byte[32];
            long l = 0;
            while (left.Read(bufLeft, 0, 32) != 0 && right.Read(bufRight, 0, 32) != 0)
            {
                l++;
                NUnit.Framework.Assert.AreEqual(bufLeft, bufRight,
                                message ?? "Streams differed at position {0} in block {1} vs {2}",
                                l,
                                bufLeft.Select(x => (char)x).ToArray(),
                                bufRight.Select(x => (char)x).ToArray()
                    );
            }
            NUnit.Framework.Assert.IsTrue(@this.Length == expectedValue.Length, message ?? "Streams were of different lengths {0} vs {1}", @this.Length, expectedValue.Length);
            return @this;
        }
    }
}