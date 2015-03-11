using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.ExampleMvc4.Controllers;
using TestBase.ExampleMvc4.Models;
using TestBase.Shoulds;

namespace TestBase.ExampleMvc.Tests.ExampleMvc4.Controllers
{
    [TestClass]
    public class When_Navigating_to_SimpleForm_controller : TestBase<SimpleFormController>
    {
        [TestInitializeAttribute]public override void SetUp() { base.SetUp(); }

        [TestMethod]
        public void SimpleForm_action_should_show_view_with_model()
        {
            UnitUnderTest.Index("string")
                .ShouldBeViewWithModel<SimpleFormModel>()
                .Name
                .ShouldBe("string");
        }
        [TestMethod]
        public void SimpleForm_action_should_show_view_with_model_bis()
        {
            UnitUnderTest.Index("string")
                .ShouldBeViewResult().ShouldHaveModel<SimpleFormModel>()
                .Name
                .ShouldBe("string");
        }
    }
}