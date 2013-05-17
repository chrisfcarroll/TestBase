using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ovineware.CodeSamples.DapperDemo.CSharp.Extensions;
using Ovineware.CodeSamples.DapperDemo.CSharp.Models;
using Ovineware.CodeSamples.DapperDemo.CSharp.Services;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Controllers
{
    public class ManagerController : Controller
    {
        private HumanResourcesService humanResourcesService;

        public ManagerController()
            : this(new HumanResourcesService())
        {
        }

        public ManagerController(HumanResourcesService humanResourcesService)
        {
            this.humanResourcesService = humanResourcesService;
        }

        public ActionResult Index()
        {
            ViewData["Employees"] = humanResourcesService.GetEmployees().OrderBy(x => x.LastName).ToSelectListItems(x => x.Id, x => String.Format("{0}, {1} {2}", x.LastName, x.FirstName, x.MiddleName));
            return View("Managers");
        }

        [HttpPost]
        public ActionResult Index(int employeeId)
        {
            IEnumerable<Manager> managers = humanResourcesService.GetManagers(employeeId);
            ViewData["Employees"] = humanResourcesService.GetEmployees().OrderBy(x => x.LastName).ToSelectListItems(x => x.Id, x => String.Format("{0}, {1} {2}", x.LastName, x.FirstName, x.MiddleName, x.Id == employeeId));
            return View("Managers", managers);
        }
    }
}
