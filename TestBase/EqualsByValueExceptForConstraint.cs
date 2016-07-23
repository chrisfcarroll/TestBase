using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace TestBase
{
    public class EqualsByValueExceptForConstraint : Constraint
    {
        private readonly object expected;
        readonly IEnumerable<string> exclusions;
        private BoolWithString compareResult;

        public EqualsByValueExceptForConstraint(object expected, IEnumerable<string> exclusions)
        {
            this.expected = expected;
            this.exclusions = exclusions;
        }

        public override bool Matches(object actual)
        {
            this.actual = actual;
            compareResult = actual.EqualsByValueOrDiffersExceptFor(expected, exclusions);
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