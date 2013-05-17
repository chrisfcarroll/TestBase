using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ovineware.CodeSamples.DapperDemo.CSharp.Services;
using Ovineware.CodeSamples.DapperDemo.CSharp.Models;
using Ovineware.CodeSamples.DapperDemo.CSharp.Extensions;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Controllers
{
    public class SubCategoryController : Controller
    {
        private ProductionService productionService;

        public SubCategoryController()
            : this(new ProductionService())
        {
        }

        public SubCategoryController(ProductionService productionService)
        {
            this.productionService = productionService;
        }

        public ActionResult Index()
        {
            IEnumerable<SubCategory> subCategories = productionService.GetSubCategories();
            return View("SubCategories", subCategories);
        }

        public ActionResult New()
        {
            ViewData["Categories"] = productionService.GetCategories().ToSelectListItems(x => x.Id, x => x.Name);
            return View("CreateOrEdit");
        }

        [HttpPost]
        public ActionResult New(SubCategory subCategory)
        {
            productionService.AddSubCategory(subCategory);
            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            SubCategory subCategory = productionService.GetSubCategory(id);
            ViewData["IsUpdate"] = true;
            ViewData["Categories"] = productionService.GetCategories().ToSelectListItems(x => x.Id, x => x.Name);
            return View("CreateOrEdit", subCategory);
        }

        [HttpPost]
        public ActionResult Update(SubCategory subCategory)
        {
            productionService.UpdateSubCategory(subCategory);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            SubCategory subCategory = productionService.GetSubCategory(id);
            if (subCategory != null)
                productionService.RemoveSubCategory(subCategory);
            return RedirectToAction("Index");
        }
    }
}
