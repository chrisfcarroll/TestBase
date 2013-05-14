using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.ExampleMvc4.Controllers;
using TestBase.Shoulds;

namespace TestBase.ExampleMvc.Tests.ExampleMvc4.Controllers
{
    [TestClass]
    public class When_navigating_to_home_controller : TestBase<HomeController>
    {
        [TestMethod]
        public void Index_action_should_show_index_page()
        {
            UnitUnderTest.Index().ShouldBeDefaultView();
        }

        [TestMethod]
        public void Index_action_should_show_the_template_generated_example_message()
        {
            var viewBag = UnitUnderTest.Index().ShouldBeViewResult().ViewBag;
            (viewBag.Message as string)
                .ShouldEqual("Modify this template to jump-start your ASP.NET MVC application.");
        }

        [TestMethod]
        public void About_action_should_show_about_page()
        {
            UnitUnderTest.About().ShouldBeDefaultView();
        }

        [TestMethod]
        public void Contact_action_should_show_contact_page()
        {
            UnitUnderTest.Contact().ShouldBeDefaultView();
        }
    }
}
