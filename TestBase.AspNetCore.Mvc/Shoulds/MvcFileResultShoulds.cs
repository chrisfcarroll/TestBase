using Microsoft.AspNetCore.Mvc;

namespace TestBase
{
    public static class MvcFileResultShoulds
    {
        public static FileResult ShouldBeFileResult(this IActionResult result, string fileDownloadName = null, string message = null,
            params object[] args)
        {
            return result
                .ShouldBeAssignableTo<FileResult>(message, args)
                .ShouldBe(
                    x => (fileDownloadName == null) || x.FileDownloadName == fileDownloadName,
                    message ?? string.Format("Expected FileResult with FileDownloadName {0}", fileDownloadName),
                    args);
        }

        public static FileContentResult ShouldBeFileContentResult(this IActionResult result, string fileDownloadName = null, string message = null,
            params object[] args)
        {
            return result
                .ShouldBeAssignableTo<FileContentResult>(message, args)
                .ShouldBe(
                    x => (fileDownloadName == null) || x.FileDownloadName == fileDownloadName,
                    message ?? string.Format("Expected FileResult with FileDownloadName {0}", fileDownloadName),
                    args);
        }

        public static FileStreamResult ShouldBeFileStreamResult(this IActionResult result, string fileDownloadName = null, string message = null,
            params object[] args)
        {
            return result
                .ShouldBeAssignableTo<FileStreamResult>(message, args)
                .ShouldBe(
                    x => (fileDownloadName == null) || x.FileDownloadName == fileDownloadName,
                    message ?? string.Format("Expected FileResult with FileDownloadName {0}", fileDownloadName),
                    args);
        }
    }
}