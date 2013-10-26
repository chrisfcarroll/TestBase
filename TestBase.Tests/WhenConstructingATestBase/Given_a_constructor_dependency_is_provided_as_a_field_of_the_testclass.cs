using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenConstructingATestBase
{
    [TestFixture]
    public class Given_a_constructor_dependency_is_provided_as_a_field_of_the_testclass : TestBase<Given_a_constructor_dependency_is_provided_as_a_field_of_the_testclass.ClassWithANotMockableConstructorDependency>
    {
        string dummy = "ConstructorDependencySpecifiedAsAField";

        [Test]
        public void Testbase_should_add_field_to_fakes_dictionary_and_use_it_in_constructing_UnitUnderTest()
        {
            UnitUnderTest.Dummy
                .ShouldBe(dummy)
                .ShouldBe(Fakes["dummy"],"Expected field dummy to be added to Fakes Dictionary")
                .ShouldBe("ConstructorDependencySpecifiedAsAField", "Expected value of field dummy to be used for dummy");
        }

        [Test]
        public void Testbase_init_should_have_created_no_mocks()
        {
            Mocks.Count().ShouldEqual(0);
        }

        public class ClassWithANotMockableConstructorDependency
        {
            public string Dummy { get; set; }

            public ClassWithANotMockableConstructorDependency(string dummy)
            {
                Dummy = dummy;
            }
        }
    }
}