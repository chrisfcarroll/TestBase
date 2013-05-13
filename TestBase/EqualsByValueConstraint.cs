using NUnit.Framework.Constraints;

namespace TestBase
{
    public class EqualsByValueConstraint : Constraint
    {
        private readonly object expected;
        private BoolWithString compareResult;

        public EqualsByValueConstraint(object expected)
        {
            this.expected = expected;
        }

        public override bool Matches(object actual)
        {
            this.actual = actual;
            compareResult = actual.EqualsByValueOrDiffers(expected);
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