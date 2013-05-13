using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class ShouldsForEqualByValue
    {
        public static T ShouldEqualByValue<T>(this T @this, T expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(@this, new EqualsByValueConstraint(expectedValue), message, args);
            return @this;
        }

        public static T ShouldEqualByValue<T>(this T @this, object expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(@this, new EqualsByValueConstraint(expectedValue), message, args);
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
                Assert.AreEqual(bufLeft, bufRight,
                                message??"Streams differed at position {0} in block {1} vs {2}",
                                l,
                                bufLeft.Select(x => (char)x).ToArray(),
                                bufRight.Select(x => (char)x).ToArray()
                    );
            }
            Assert.IsTrue(@this.Length == expectedValue.Length, message??"Streams were of different lengths {0} vs {1}", @this.Length, expectedValue.Length);
            return @this;
        }

        public static bool IsEqualByValue(this Stream @this, Stream expectedValue, [Optional] string message, params object[] args)
        {
            @this.Position = 0;
            expectedValue.Position = 0;
            var left = new BufferedStream(@this);
            var right = new BufferedStream(expectedValue);
            byte[] bufLeft = new byte[1];
            byte[] bufRight = new byte[1];
            long l = 0;

            while (left.Read(bufLeft, 0, 1) != 0 && right.Read(bufRight, 0, 1) != 0)
            {
                l++;
                if (bufLeft[0] != bufRight[0]) return false;
            }

            return @this.Length == expectedValue.Length;
        }
    }
}