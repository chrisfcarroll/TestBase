using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenConstructingATestBase
{
    [TestFixture]
    public class For_a_class_with_no_constructor_parameters : TestBase<For_a_class_with_no_constructor_parameters.ClassWithNoConstructorDependency>
    {
        [Test]
        public void Testbase_should_instantiate_it_before_a_testmethod_runs()
        {
            UnitUnderTest.ShouldNotBeNull().ShouldBeOfType<ClassWithNoConstructorDependency>();
        }

        public class ClassWithNoConstructorDependency
        {
        }
    }
}