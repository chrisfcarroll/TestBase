using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.TestBaseAutoMockTests.WhenConstructingATestBase
{
    [Ignore("Will not fix in TestBase 3. Use TestBase4.AutoBuild instead")]
    public class Given_a_recursive_constructor_dependency_is_provided_as_a_field_of_the_testclass : TestBase<Given_a_recursive_constructor_dependency_is_provided_as_a_field_of_the_testclass.ClassWithConstructorDependency>
    {
        string dummy = "ConstructorDependencySpecifiedAsAField";

        [Ignore]
        public void Testbase_should_add_field_to_fakes_dictionary_and_use_it_in_constructing_UnitUnderTest()
        {
            UnitUnderTest.Dependency1.Dummy
                .ShouldBe(dummy)
                .ShouldBe(Fakes["dummy"],"Expected field dummy to be added to Fakes Dictionary")
                .ShouldBe("ConstructorDependencySpecifiedAsAField", "Expected value of field dummy to be used for dummy");
        }

        [Ignore]
        public void Testbase_init_should_have_created_no_mocks()
        {
            Mocks.Count().ShouldEqual(0);
        }

        public class ClassDependentOnADummy
        {
            public string Dummy { get; set; }

            public ClassDependentOnADummy(string dummy)
            {
                Dummy = dummy;
            }
        }

        public class ClassWithConstructorDependency
        {
            public ClassDependentOnADummy Dependency1 { get; set; }

            public ClassWithConstructorDependency(ClassDependentOnADummy dependency1)
            {
                Dependency1 = dependency1;
            }
        }
    }
}