using System.IO;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ShouldsFeedbackWhenAsserting.ShouldReturnTheActualBeingAsserted__Given_AssertionPass
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

        [Test]
        public void ShouldBeFileContentResult_Should_return_fileresult()
        {
            var fileContentResult = new FileContentResult(new byte[] { 1 }, "fake/type");
            var fileDownloadName = "FakeDownloadName";
            fileContentResult.FileDownloadName = fileDownloadName;

            fileContentResult.ShouldBeFileContentResult(fileDownloadName).ShouldEqual(fileContentResult);
        }

        [Test]
        public void ShouldBeFileStreamResult_Should_return_filestreamresult()
        {
            var fileStreamResult = new FileStreamResult(new MemoryStream(new byte[] { 1 }), "fake/type");
            var fileDownloadName = "FakeDownloadName";
            fileStreamResult.FileDownloadName = fileDownloadName;

            fileStreamResult.ShouldBeFileStreamResult(fileDownloadName).ShouldEqual(fileStreamResult);
        }
    }
}
