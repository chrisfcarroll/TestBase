using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ovineware.CodeSamples.DapperDemo.CSharp.Repositories;
using Ovineware.CodeSamples.DapperDemo.CSharp.Models;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Services
{
    public class ProductionService
    {
        private ProductionRepository productionRepository;

        public ProductionService()
            : this(new ProductionRepository())
        {
        }

        public ProductionService(ProductionRepository productRepository)
        {
            this.productionRepository = productRepository;
        }

        public IEnumerable<Product> GetProducts()
        {
            return productionRepository.SelectProducts();
        }

        public IEnumerable<Product> GetProductsWithSubCategories()
        {
            return productionRepository.SelectProductsWithSubCategories();
        }

        public Product GetProduct(int productId)
        {
            return productionRepository.SelectProduct(productId);
        }

        public void AddProduct(Product product)
        {
            product.ModifiedDate = DateTime.Now;
            productionRepository.InsertProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            product.ModifiedDate = DateTime.Now;
            productionRepository.UpdateProduct(product);
        }

        public void RemoveProduct(Product product)
        {
            productionRepository.DeleteProduct(product);
        }

        public byte[] GetImage(int productId, ImageSize size)
        {
            switch (size)
            {
                case ImageSize.Large: return productionRepository.SelectPhoto(productId);
                case ImageSize.Thumbnail: return productionRepository.SelectThumbnail(productId);
                default: throw new InvalidOperationException("Invalid image size.");
            }
        }

        public IEnumerable<MeasurementUnit> GetMeasurementUnits()
        {
            return productionRepository.SelectMeasurementUnits();
        }

        public IEnumerable<Category> GetCategories()
        {
            return productionRepository.SelectCategories();
        }

        public IEnumerable<SubCategory> GetSubCategories()
        {
            return productionRepository.SelectSubCategories();
        }

        public SubCategory GetSubCategory(int subCategoryId)
        {
            return productionRepository.SelectSubCategory(subCategoryId);
        }

        public void AddSubCategory(SubCategory subCategory)
        {
            subCategory.ModifiedOn = DateTime.Now;
            productionRepository.InsertSubCategory(subCategory);
        }

        public void UpdateSubCategory(SubCategory subCategory)
        {
            subCategory.ModifiedOn = DateTime.Now;
            productionRepository.UpdateSubCategory(subCategory);
        }

        public void RemoveSubCategory(SubCategory subCategory)
        {
            productionRepository.DeleteSubCategory(subCategory);
        }

        public IEnumerable<Model> GetModels()
        {
            return productionRepository.SelectModels();
        }
    }
}