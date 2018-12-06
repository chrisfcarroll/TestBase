using System.Text;
using NUnit.Framework;
using TestBase;
using TestBase.Shoulds;

namespace TestBaseMvc.Tests
{
    [TestFixture]
    public class FileResultShoulds
    {
        [Test]
        public void ShouldBeFileContentResult_ShouldAssertContentTypeAndDownloadFileName()
        {
            var controllerUnderTest = new ATestController(new IDependency()).WithHttpContextAndRoutes();

            var result =
            controllerUnderTest.AFileResult("my words", "text/plain", "words.txt")
                               .ShouldBeFileContentResult();

            result.FileContents.ShouldEqualByValue(Encoding.UTF8.GetBytes("my words"));
            result.ContentType.ShouldBe("text/plain");
            result.FileDownloadName.ShouldBe("words.txt");
        }

        [Test]
        public void ShouldBeFileResult_ShouldAssertContentTypeAndDownloadFileName()
        {
            var controllerUnderTest = new ATestController(new IDependency()).WithHttpContextAndRoutes();

            var result =
            controllerUnderTest.AFileResult("my words", "text/plain", "words.txt")
                               .ShouldBeFileResult();

            result.ContentType.ShouldBe("text/plain");
            result.FileDownloadName.ShouldBe("words.txt");
        }
    }
}
