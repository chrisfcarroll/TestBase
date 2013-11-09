using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting.ShouldReturnTheActualBeingAsserted__Given_AssertionPass
{
    [TestFixture]
    public class ShouldReturn__TestCases_MvcActionResults
    {
        [Test]
        public void ShouldBeFileResult_Should_return_fileresult()
        {
            var fileResult = new FileContentResult(new byte[] {1}, "fake/type");
            var fileDownloadName = "FakeDownloadName";
            fileResult.FileDownloadName = fileDownloadName;

            fileResult.ShouldBeFileResult(fileDownloadName).ShouldEqual(fileResult);
        }
    }
}
