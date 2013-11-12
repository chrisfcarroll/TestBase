using System.Web;
using Moq;

namespace TestBase.MockHttpContext
{
    public static class HttpServerUtilityBaseExtensions
    {
        private const string ApplicationWebContentRootDirectory = "..\\..\\..\\Web";

        public static void SimpleMapPath(this Mock<HttpServerUtilityBase> @this)
        {
            @this.Setup(
                x => x.MapPath(It.IsAny<string>())
                ).Returns(
                    (string s) => s.Replace('/', '\\')
                );
        }

        public static void MapPathToApplicationWebProjectDirectory(this Mock<HttpServerUtilityBase> @this)
        {

            @this.Setup(
                x => x.MapPath(It.IsAny<string>())
                ).Returns(
                    (string s) => ApplicationWebContentRootDirectory + s.Replace('/', '\\')
                );
        }
    }
}