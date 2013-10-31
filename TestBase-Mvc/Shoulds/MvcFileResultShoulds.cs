using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class MvcFileResultShoulds
    {
        public static FileResult ShouldBeFileResult(this ActionResult result, string fileDownloadName = null, string message=null, params object[] args)
        {
            return result
                .ShouldBeAssignableTo<FileResult>(message, args)
                .ShouldHave(
                        x=> (fileDownloadName==null) || x.FileDownloadName==fileDownloadName, 
                        message?? string.Format("Expected FileResult with FileDownloadName {0}", fileDownloadName),
                        args);
        }
    }
}
