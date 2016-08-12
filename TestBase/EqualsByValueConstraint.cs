using NUnit.Framework.Constraints;

namespace TestBase
{
    public class EqualsByValueConstraint : Constraint
    {
        readonly object expected;
        BoolWithString compareResult;
        readonly double tolerance;

        public EqualsByValueConstraint(object expected)
        {
            this.expected = expected;
            this.tolerance = 0;
        }
        public EqualsByValueConstraint(object expected, double floatTolerance)
        {
            this.expected = expected;
            this.tolerance = floatTolerance;
        }

        public override bool Matches(object actual)
        {
            this.actual = actual;
            compareResult = actual.EqualsByValueOrDiffers(expected, tolerance);
            return compareResult;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteExpectedValue(this.expected);
        }
        public override void WriteActualValueTo(MessageWriter writer)
        {
            base.WriteActualValueTo(writer);
            writer.WriteLine();
            writer.WriteMessageLine("Compare Result " + compareResult);
        }
    }
}