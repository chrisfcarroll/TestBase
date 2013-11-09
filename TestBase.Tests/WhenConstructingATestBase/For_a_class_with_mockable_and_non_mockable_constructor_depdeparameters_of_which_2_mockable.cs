using Moq;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenConstructingATestBase
{
    [TestFixture]
    public class For_a_class_with_both_mockable_and_non_mockable_constructor_dependencies : TestBase<For_a_class_with_both_mockable_and_non_mockable_constructor_dependencies.ClassWith4ConstructorDependenciesOfWhich2AreMockable>
    {
        [Test]
        public void Testbase_should_instantiate_it_before_a_testmethod_runs()
        {
            UnitUnderTest.ShouldNotBeNull().ShouldBeOfType<ClassWith4ConstructorDependenciesOfWhich2AreMockable>();
        }

        [Test]
        public void Testbase_init_should_have_put_the_mocks_in_the_Mocks_dictionary()
        {
            Mocks.Get<object>().ShouldEqual(Mock.Get(UnitUnderTest.MockableType1));
            Mocks.Get<For_a_class_with_no_constructor_parameters.ClassWithNoConstructorDependency>()
                .ShouldEqual(Mock.Get(UnitUnderTest.MockableType2));
        }

        [Test]
        public void UnitUnderTest_should_have_got_an_autofaked_string_dependency_set_to_AutoFakePrefix_plus_dependency_name()
        {
            UnitUnderTest.UnmockableType.ShouldEqual(AutoFakePrefix + "unmockableType");
        }

        [Test]
        public void UnitUnderTest_should_have_got_an_autofaked_valuetype_dependency_set_to_defaultvalue()
        {
            UnitUnderTest.ValueType.ShouldEqual(default(int));
        }

        [Test]
        public void UnitUnderTest_should_have_got_the_automocked_dependencies_as_mocks()
        {
            UnitUnderTest.MockableType1.ShouldBe(Mocks.Get<object>().Object);
            UnitUnderTest.MockableType2.ShouldBe(Mocks.Get<For_a_class_with_no_constructor_parameters.ClassWithNoConstructorDependency>().Object);
        }

        public class ClassWith4ConstructorDependenciesOfWhich2AreMockable
        {
            public string UnmockableType { get; set; }
            public int ValueType { get; set; }
            public object MockableType1 { get; set; }
            public For_a_class_with_no_constructor_parameters.ClassWithNoConstructorDependency MockableType2 { get; set; }

            public ClassWith4ConstructorDependenciesOfWhich2AreMockable(
                        string unmockableType, 
                        int valueType, 
                        object mockableType1, 
                        For_a_class_with_no_constructor_parameters.ClassWithNoConstructorDependency mockableType2)
            {
                UnmockableType = unmockableType;
                ValueType = valueType;
                MockableType1 = mockableType1;
                MockableType2 = mockableType2;
            }
        }
    }


}