using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenConstructingATestBase
{
    [TestFixture]
    public class For_a_class_with_1_constructor_parameter : TestBase<For_a_class_with_1_constructor_parameter.ClassWithANotMockableConstructorDependency>
    {
        [Test]
        public void Testbase_should_instantiate_it_before_a_testmethod_runs()
        {
            UnitUnderTest.ShouldNotBeNull().ShouldBeOfType<ClassWithANotMockableConstructorDependency>();
        }

        [Test]
        public void Testbase_init_should_have_created_no_mocks()
        {
            Mocks.Count().ShouldEqual(0);
        }

        public class ClassWithANotMockableConstructorDependency
        {
            public ClassWithANotMockableConstructorDependency(string dummy)
            {
            }
        }
    }
}