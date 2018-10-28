using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests
{
    [TestFixture]
    public class WhenComparingObjectsWithPublicFieldsByValue
    {
        public class FieldsClass
        {
            public int Id;
            public string Name;
        }

        readonly FieldsClass object1 = new FieldsClass
        {
            Id = 1,
            Name = "1",
        };
        readonly FieldsClass object1again = new FieldsClass
        {
            Id = 1,
            Name = "1",
        };
        readonly FieldsClass object2 = new FieldsClass
        {
            Id = 2,
            Name = "2",
        };
        readonly FieldsClass object3 = new FieldsClass
        {
            Id = 1,
            Name = "1B",
        };

        [Test]
        public void Should_return_true_when_the_same()
        {
            object1.EqualsByValue(object1again).ShouldBeTrue();
            object1.EqualsByValueOrDiffersExceptFor(object1again, new[] {""}).AsBool.ShouldBeTrue();
        }

        [Test]
        public void Should_return_false_when_not_the_same()
        {
            object1.EqualsByValue(object2).ShouldBeFalse("Failed to distinguish object1 from object 2");
            object1.EqualsByValue(object3).ShouldBeFalse("Failed to distinguish object1 from object 3");
        }
    }
}
