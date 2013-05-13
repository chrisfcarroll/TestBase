using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenConstructingATestBase
{
    [TestClass]
    public class For_a_class_with_no_constructor_parameters : TestBase<ClassWithNoConstructorDependency>
    {
        [TestMethod]
        public void Testbase_should_instantiate_it_before_a_testmethod_runs()
        {
            UnitUnderTest.ShouldNotBeNull().ShouldBeOfType<ClassWithNoConstructorDependency>();
        }
    }

    [TestClass]
    public class For_a_class_with_1_constructor_parameter : TestBase<ClassWith1ConstructorDependency>
    {
        [TestMethod]
        public void Testbase_should_instantiate_it_before_a_testmethod_runs()
        {
            UnitUnderTest.ShouldNotBeNull().ShouldBeOfType<ClassWith1ConstructorDependency>();
        }

        [TestMethod]
        public void Testbase_init_should_have_created_no_mocks()
        {
            MockDependencies.Count().ShouldEqual(0);
        }
    }

    [TestClass]
    public class For_a_class_with_4_constructor_parameters_of_which_2_mockable : TestBase<ClassWith4ConstructorDependenciesOfWhich2AreMockable>
    {
        [TestMethod]
        public void Testbase_should_instantiate_it_before_a_testmethod_runs()
        {
            UnitUnderTest.ShouldNotBeNull().ShouldBeOfType<ClassWith4ConstructorDependenciesOfWhich2AreMockable>();
        }

        [TestMethod]
        public void Testbase_init_should_have_created_2_mocks()
        {
            MockDependencies.Count().ShouldEqual(2);
        }
    }

    public class ClassWithNoConstructorDependency
    {
    }

    public class ClassWith1ConstructorDependency
    {
        public ClassWith1ConstructorDependency(string dummy)
        {
        }
    }

    public class ClassWith4ConstructorDependenciesOfWhich2AreMockable
    {
        public ClassWith4ConstructorDependenciesOfWhich2AreMockable(string unmockableType, int valueType, object mockableType1,
                                                ClassWithNoConstructorDependency mockableType2)
        {
        }
    }
}