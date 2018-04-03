using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TestBase
{
    public static class StreamShoulds
    {
        public static int MaxExpectedBytesToShowInFailureOutput=30;
        
        /// <summary>Assert this <paramref name="@this"/>, considered as a byte[], contains <paramref name="expected"/> as a substring.</summary>
        /// <returns>@this</returns>
        public static Stream ShouldContain(this Stream @this, Stream expected, string message=null, params object[] args)
        {
            Assert.That(@this, actual=>StreamContains(actual,expected), message ?? $"{nameof(StreamShoulds)}{nameof(ShouldContain)}", args);
            return @this;
        }
        
        /// <summary>Assert this <paramref name="@this"/>, considered as a byte[], contains <paramref name="expected"/> as a substring.</summary>
        /// <returns>@this</returns>
        public static Stream ShouldContain(this Stream @this, byte[] expected, string message=null, params object[] args)
        {
            Assert.That(@this, 
                        actual=>StreamContains(actual,expected), 
                        message ?? $"{nameof(StreamShoulds)}{nameof(ShouldContain)} {expected.Take(MaxExpectedBytesToShowInFailureOutput).Select(x=>(char)x).ToArray()}", args);
            return @this;
        }

        /// <summary>Synonym for <seealso cref="ShouldHaveSameStreamContentAs{T,TE}"/> Assert this <paramref name="@this"/> and <paramref name="expected"/> are byte-for-byte identical</summary>
        /// <returns>@this</returns>
        public static T ShouldEqualByStreamContent<T,TE>(this T @this, TE expected, string message = null, params object[] args) where T: Stream where TE:Stream
        {
            return ShouldHaveSameStreamContentAs(@this, expected, message, args);
        }

        /// <summary>Assert this <paramref name="@this"/> and <paramref name="expected"/> are byte-for-byte identical</summary>
        /// <returns>@this</returns>
        public static T ShouldHaveSameStreamContentAs<T,TE>(this T @this, TE expected, string message = null, params object[] args) where T: Stream where TE:Stream
        {
            @this.Position = 0;
            expected.Position = 0;
            var left = new BufferedStream(@this);
            var right = new BufferedStream(expected);
            byte[] bufLeft = new byte[32];
            byte[] bufRight = new byte[32];
            long l = 0;
            var lbytesread = 0;
            var rbytesread = 0;
            while ( (lbytesread=left.Read(bufLeft, 0, MaxExpectedBytesToShowInFailureOutput)) != 0 && (rbytesread=right.Read(bufRight, 0, MaxExpectedBytesToShowInFailureOutput)) != 0)
            {
                var mismatches = Enumerable.Range(0, Math.Min(lbytesread, rbytesread)).Where(i => bufLeft[i] != bufRight[i]).ToArray();
                if (mismatches.Length > 0)
                {
                    Assert.That(mismatches.Length == 0,
                                message
                             ?? nameof(ShouldHaveSameStreamContentAs)
                              + string.Format("Streams differed starting at position {0}: {1} vs {2}",
                                              l * MaxExpectedBytesToShowInFailureOutput + mismatches.First(),
                                              bufLeft.Skip(mismatches[0]).Select(x => (char) x).ToArray(),
                                              bufRight.Skip(mismatches[0]).Select(x => (char) x).ToArray()),
                                args
                               );
                }

                l++;
            }

            Assert.That(@this.Length == expected.Length,
                        message ?? $"Streams were of different lengths {@this.Length} vs {expected.Length}",
                        args);
            return @this;
        }
        
        public static bool StreamContains(Stream actual, Stream expected)
        {
            var expectedBytes = new byte[expected.Length];
            expected.Position = 0;
            expected.Read(expectedBytes, 0, expectedBytes.Length);
            return StreamContains(actual, expectedBytes);
        }

        public static bool StreamContains(Stream actual, byte[] expectedContent)
        {
            actual.Position = 0;
            using (var left = new BufferedStream(actual,expectedContent.Length))
            {
                byte[] bufLeft = new byte[expectedContent.Length];
                for (int i = 0; i < expectedContent.Length; i++)
                {
                    left.Position = i;
                    left.Read(bufLeft, 0, expectedContent.Length);
                    if (bufLeft.EqualsByValue(expectedContent))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }    
}