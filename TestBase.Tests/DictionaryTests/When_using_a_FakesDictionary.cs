using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Shoulds;

namespace TestBase.Tests.DictionaryTests
{
    [TestClass]
    public class When_using_a_FakesDictionary : TestBase<FakesDictionary>
    {
        [TestMethod]
        public void Get_should_be_strongly_typed()
        {
            UnitUnderTest.Add("", "");
            UnitUnderTest.Get<string>("").ShouldEqual("");

            try
            {
                UnitUnderTest.Add("1", 1);
                UnitUnderTest.Get<string>("1").ShouldEqual(1);
                Assert.Fail("Method should have thrown on the line above");
            }
            catch (InvalidCastException e)
            {
                //test pass
            }
        }

        [TestMethod]
        public void Add_followed_by_Get_Should_return_Original()
        {
            UnitUnderTest.Add("", "");
            UnitUnderTest.Get<string>("").ShouldEqual("");

            UnitUnderTest.Add("1", 1);
            UnitUnderTest.Get<int>("1").ShouldEqual(1);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Add_followed_by_Remove_followed_by_Get_Should_Throw()
        {
            UnitUnderTest.Add("", "");
            UnitUnderTest.Remove("");
            UnitUnderTest.Get<string>("");
        }
    }
}
