using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework.Constraints;

namespace TestBase
{
    public class StreamContainsConstraint : Constraint
    {
        private readonly byte[] expectedBytes;
        private StringBuilder actualTruncated;

        public StreamContainsConstraint(Stream expected)
        {
            this.expectedBytes = new byte[expected.Length];
            expected.Position = 0;
            expected.Read(expectedBytes, 0, expectedBytes.Length);
        }

        public StreamContainsConstraint(byte[] expectedValue)
        {
            expectedBytes = expectedValue;
        }

        public override bool Matches(object actual)
        {
            this.actual = actual;
            return StreamContains((Stream)actual, expectedBytes);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteExpectedValue(this.expectedBytes.Take(20));
        }
        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteActualValue(actualTruncated.ToString());
        }

        public bool StreamContains(Stream actual, byte[] expectedContent)
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

                    if (i == 0)
                    {
                        actualTruncated = TruncateToStringBuilder(bufLeft, 20);
                    }
                }
            }
            return false;
        }

        public static StringBuilder TruncateToStringBuilder(byte[] bufLeft, int maxLength)
        {
            var x = new StringBuilder();
            x.Append(bufLeft.Take(maxLength).Select(b => (char) (int) b).ToArray());
            return x;
        }
    }
}