using System;

namespace TestBase
{
    public static class MockHttpContextMonoAppDomainValues
    {
        /// <summary>
        /// Used by <see cref="MockHttpContextHelper"/> to ensure values required to
        /// stub or mock <see cref="System.Web.HttpContext"/> when running on mono:
        /// <c>AppDomain.CurrentDomain.SetData(".appPath", AppDomain.CurrentDomain.BaseDirectory)</c>
        /// <c>AppDomain.CurrentDomain.SetData(".appVPath", "/")</c>
        /// </summary>
        public static void Ensure()
        {
            try{AppDomain.CurrentDomain.SetData(".appPath", AppDomain.CurrentDomain.BaseDirectory);}catch{}
            try{AppDomain.CurrentDomain.SetData(".appVPath", "/");}catch{}
        }
    }
}