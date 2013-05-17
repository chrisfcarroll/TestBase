using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Ovineware.CodeSamples.DapperDemo.CSharp.Extensions;
using Ovineware.CodeSamples.DapperDemo.CSharp.Models;
using Ovineware.CodeSamples.DapperDemo.CSharp.Services;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Controllers
{
    public class ProductController : Controller
    {
        private ProductionService productionService;

        public ProductController()
            : this(new ProductionService())
        {
        }

        public ProductController(ProductionService productionService)
        {
            this.productionService = productionService;
        }

        public ActionResult Index()
        {
            IEnumerable<Product> products = productionService.GetProducts();
            return View("Products", products);
        }

        public ActionResult WithSubCategories()
        {
            IEnumerable<Product> products = productionService.GetProductsWithSubCategories();
            return View("Products", products);
        }

        public void Image(int id, ImageSize size)
        {
            byte[] image = productionService.GetImage(id, size);
            if (image != null && image.Length > 0)
            {
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("content-length", image.Length.ToString());
                Response.ContentType = "image/jpeg";
                Response.BinaryWrite(image);
                Response.Flush();
            }
            else
                throw new HttpException(404, "The image could not be found");      
        }

        public ActionResult New()
        {
            ViewData["MeasurementUnits"] = productionService.GetMeasurementUnits().ToSelectListItems(x => x.Code, x => x.Name);
            ViewData["SubCategories"] = productionService.GetSubCategories().ToSelectListItems(x => x.Id, x => x.Name);
            ViewData["Models"] = productionService.GetModels().ToSelectListItems(x => x.Id, x => x.Name);
            return View("CreateOrEdit");
        }

        [HttpPost]
        public ActionResult New(Product product)
        {
            productionService.AddProduct(product);
            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            Product product = productionService.GetProduct(id);
            ViewData["IsUpdate"] = true;
            ViewData["MeasurementUnits"] = productionService.GetMeasurementUnits().ToSelectListItems(x => x.Code, x => x.Name);
            ViewData["SubCategories"] = productionService.GetSubCategories().ToSelectListItems(x => x.Id, x => x.Name);
            ViewData["Models"] = productionService.GetModels().ToSelectListItems(x => x.Id, x => x.Name);
            return View("CreateOrEdit", product);
        }        

        [HttpPost]
        public ActionResult Update(Product product)
        {
            productionService.UpdateProduct(product);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            Product product = productionService.GetProduct(id);
            if (product != null)
                productionService.RemoveProduct(product);
            return RedirectToAction("Index");
        }
    }
}
