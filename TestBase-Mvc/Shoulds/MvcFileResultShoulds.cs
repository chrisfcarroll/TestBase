using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace TestBase.Shoulds
{
    public static class MvcFileResultShoulds
    {
        public static FileResult ShouldBeFileResult(this ActionResult result, string fileDownloadName = null, string message = null,
            params object[] args)
        {
            return result
                .ShouldBeAssignableTo<FileResult>(message, args)
                .ShouldHave(
                    x => (fileDownloadName == null) || x.FileDownloadName == fileDownloadName,
                    message ?? string.Format("Expected FileResult with FileDownloadName {0}", fileDownloadName),
                    args);
        }

        public static FileContentResult ShouldBeFileContentResult(this ActionResult result, string fileDownloadName = null, string message = null,
            params object[] args)
        {
            return result
                .ShouldBeAssignableTo<FileContentResult>(message, args)
                .ShouldHave(
                    x => (fileDownloadName == null) || x.FileDownloadName == fileDownloadName,
                    message ?? string.Format("Expected FileResult with FileDownloadName {0}", fileDownloadName),
                    args);
        }

        public static FileStreamResult ShouldBeFileStreamResult(this ActionResult result, string fileDownloadName = null, string message = null,
            params object[] args)
        {
            return result
                .ShouldBeAssignableTo<FileStreamResult>(message, args)
                .ShouldHave(
                    x => (fileDownloadName == null) || x.FileDownloadName == fileDownloadName,
                    message ?? string.Format("Expected FileResult with FileDownloadName {0}", fileDownloadName),
                    args);
        }
    }
}