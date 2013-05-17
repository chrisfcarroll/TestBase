using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Ovineware.CodeSamples.DapperDemo.CSharp.Models;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Repositories
{
    public class ProductionRepository : BaseRepository
    {
        public IEnumerable<Product> SelectProducts()
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT ProductID, Name, ProductNumber, MakeFlag, FinishedGoodsFlag, Color, SafetyStockLevel, ReorderPoint, StandardCost, ListPrice, Size, SizeUnitMeasureCode, WeightUnitMeasureCode, Weight, DaysToManufacture, ProductLine, Class, Style, ProductSubcategoryID, ProductModelID, SellStartDate, SellEndDate, DiscontinuedDate, ModifiedDate " +
                                     "FROM Production.Product";
                return connection.Query<Product>(query);
            }
        }

        public IEnumerable<Product> SelectProductsWithSubCategories()
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT p.ProductID, p.Name, p.ProductNumber, p.MakeFlag, p.FinishedGoodsFlag, p.Color, p.SafetyStockLevel, p.ReorderPoint, p.StandardCost, p.ListPrice, p.Size, p.SizeUnitMeasureCode, p.WeightUnitMeasureCode, p.Weight, p.DaysToManufacture, p.ProductLine, p.Class, p.Style, p.ProductSubcategoryID, p.ProductModelID, p.SellStartDate, p.SellEndDate, p.DiscontinuedDate,  p.ModifiedDate, " +
                                     "s.ProductSubcategoryId AS Id, s.ProductCategoryID AS CategoryId, s.[Name], s.ModifiedDate AS ModifiedOn " +
                                     "FROM Production.Product p " +
                                     "LEFT OUTER JOIN Production.ProductSubcategory s ON s.ProductSubcategoryId = p.ProductSubcategoryID";
                return connection.Query<Product, SubCategory, Product>(query, (product, subCategory) => { product.SubCategory = subCategory; return product; });
            }
        }

        public Product SelectProduct(int productId)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT ProductID, Name, ProductNumber, MakeFlag, FinishedGoodsFlag, Color, SafetyStockLevel, ReorderPoint, StandardCost, ListPrice, Size, SizeUnitMeasureCode, WeightUnitMeasureCode, Weight, DaysToManufacture, ProductLine, Class, Style, ProductSubcategoryID, ProductModelID, SellStartDate, SellEndDate, DiscontinuedDate, rowguid, ModifiedDate " +
                                     "FROM Production.Product " +
                                     "WHERE ProductID = @ProductId";                               
                return connection.Query<Product>(query, new { ProductId = productId }).SingleOrDefault();
            }
        }

        public int InsertProduct(Product product)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "INSERT INTO Production.Product(Name, ProductNumber, MakeFlag, FinishedGoodsFlag, Color, SafetyStockLevel, ReorderPoint, StandardCost, ListPrice, Size, SizeUnitMeasureCode, WeightUnitMeasureCode, Weight, DaysToManufacture, ProductLine, Class, Style, ProductSubcategoryID, ProductModelID, SellStartDate, SellEndDate, DiscontinuedDate) " +
                                     "VALUES (@Name, @ProductNumber, @MakeFlag, @FinishedGoodsFlag, @Color, @SafetyStockLevel, @ReorderPoint, @StandardCost, @ListPrice, @Size, @SizeUnitMeasureCode, @WeightUnitMeasureCode, @Weight, @DaysToManufacture, @ProductLine, @Class, @Style, @ProductSubcategoryID, @ProductModelID, @SellStartDate, @SellEndDate, @DiscontinuedDate)";
                var parameters = new
                {
                    Name = product.Name,
                    ProductNumber = product.ProductNumber,
                    MakeFlag = product.MakeFlag,
                    FinishedGoodsFlag = product.FinishedGoodsFlag,
                    Color = product.Color,
                    SafetyStockLevel = product.SafetyStockLevel,
                    ReorderPoint = product.ReorderPoint,
                    StandardCost = product.StandardCost,
                    ListPrice = product.ListPrice,
                    Size = product.Size,
                    SizeUnitMeasureCode = product.SizeUnitMeasureCode,
                    WeightUnitMeasureCode = product.WeightUnitMeasureCode,
                    Weight = product.Weight,
                    DaysToManufacture = product.DaysToManufacture,
                    ProductLine = product.ProductLine,
                    Class = product.Class,
                    Style = product.Style,
                    ProductSubcategoryID = product.ProductSubcategoryID,
                    ProductModelID = product.ProductModelID,
                    SellStartDate = product.SellStartDate,
                    SellEndDate = product.SellEndDate,
                    DiscontinuedDate = product.DiscontinuedDate
                };
                int rowsAffected = connection.Execute(query, parameters);
                SetIdentity<int>(connection, id => product.ProductID = id);
                return rowsAffected;
            }
        }

        public int UpdateProduct(Product product)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "UPDATE Production.Product " +
                                     "SET Name = @Name, " +
                                     "ProductNumber = @ProductNumber, " +
                                     "MakeFlag = @MakeFlag, " +
                                     "FinishedGoodsFlag = @FinishedGoodsFlag, " +
                                     "Color = @Color, " +
                                     "SafetyStockLevel = SafetyStockLevel, " +
                                     "ReorderPoint = @ReorderPoint, " +
                                     "StandardCost = @StandardCost, " +
                                     "ListPrice = @ListPrice, " +
                                     "Size = @Size, " +
                                     "SizeUnitMeasureCode = @SizeUnitMeasureCode, " +
                                     "WeightUnitMeasureCode = @WeightUnitMeasureCode, " +
                                     "Weight = @Weight, " +
                                     "DaysToManufacture = @DaysToManufacture, " +
                                     "ProductLine = @ProductLine, " +
                                     "Class = @Class, " +
                                     "Style = @Style, " +
                                     "ProductSubcategoryID = @ProductSubcategoryID, " +
                                     "ProductModelID = @ProductModelID, " +
                                     "SellStartDate = @SellStartDate, " +
                                     "SellEndDate = @SellEndDate, " +
                                     "DiscontinuedDate = @DiscontinuedDate, " +
                                     "ModifiedDate = @ModifiedDate " +
                                     "WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = product.ProductID,
                    Name = product.Name,
                    ProductNumber = product.ProductNumber,
                    MakeFlag = product.MakeFlag,
                    FinishedGoodsFlag = product.FinishedGoodsFlag,
                    Color = product.Color,
                    SafetyStockLevel = product.SafetyStockLevel,
                    ReorderPoint = product.ReorderPoint,
                    StandardCost = product.StandardCost,
                    ListPrice = product.ListPrice,
                    Size = product.Size,
                    SizeUnitMeasureCode = product.SizeUnitMeasureCode,
                    WeightUnitMeasureCode = product.WeightUnitMeasureCode,
                    Weight = product.Weight,
                    DaysToManufacture = product.DaysToManufacture,
                    ProductLine = product.ProductLine,
                    Class = product.Class,
                    Style = product.Style,
                    ProductSubcategoryID = product.ProductSubcategoryID,
                    ProductModelID = product.ProductModelID,
                    SellStartDate = product.SellStartDate,
                    SellEndDate = product.SellEndDate,
                    DiscontinuedDate = product.DiscontinuedDate,
                    ModifiedDate = product.ModifiedDate
                };
                return connection.Execute(query, parameters);
            }
        }

        public int DeleteProduct(Product product)
        {
            using (IDbConnection connection = OpenConnection())
            {                
                const string deleteImageQuery = "DELETE FROM Production.ProductProductPhoto " +
                                                "WHERE ProductID = @ProductID";
                const string deleteProductQuery = "DELETE FROM Production.Product " +
                                                  "WHERE ProductID = @ProductID";
                IDbTransaction transaction = connection.BeginTransaction();
                int rowsAffected = connection.Execute(deleteImageQuery, new { ProductID = product.ProductID }, transaction);
                rowsAffected += connection.Execute(deleteProductQuery, new { ProductID = product.ProductID }, transaction);
                transaction.Commit();
                return rowsAffected;
            }
        }

        public byte[] SelectThumbnail(int productId)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT pp.ThumbNailPhoto " +
                                     "FROM Production.ProductPhoto pp " +
                                     "INNER JOIN Production.ProductProductPhoto ppp ON ppp.ProductPhotoID = pp.ProductPhotoID " +
                                     "WHERE ppp.ProductID = @ProductId";
                dynamic result = connection.Query(query, new { ProductId = productId }).SingleOrDefault();
                return result != null ? result.ThumbNailPhoto : null;
            }
        }

        public byte[] SelectPhoto(int productId)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT pp.LargePhoto " +
                                     "FROM Production.ProductPhoto pp " +
                                     "INNER JOIN Production.ProductProductPhoto ppp ON ppp.ProductPhotoID = pp.ProductPhotoID " +
                                     "WHERE ppp.ProductID = @ProductId";
                dynamic result = connection.Query(query, new { ProductId = productId }).SingleOrDefault();
                return result != null ? result.LargePhoto : null;
            }
        }

        public IEnumerable<MeasurementUnit> SelectMeasurementUnits()
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT UnitMeasureCode AS [Code], [Name], ModifiedDate as ModifiedOn " +
                                     "FROM Production.UnitMeasure";
                return connection.Query<MeasurementUnit>(query);
            }
        }

        public IEnumerable<Category> SelectCategories()
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT ProductCategoryId AS Id, [Name], ModifiedDate AS ModifiedOn " +
                                     "FROM Production.ProductCategory";
                return connection.Query<Category>(query);
            }
        }

        public IEnumerable<SubCategory> SelectSubCategories()
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT ProductSubcategoryId AS Id, ProductCategoryID AS CategoryId, [Name], ModifiedDate AS ModifiedOn " +
                                     "FROM Production.ProductSubcategory";
                return connection.Query<SubCategory>(query);
            }
        }

        public SubCategory SelectSubCategory(int subCategoryId)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT ProductSubcategoryId AS Id, ProductCategoryID AS CategoryId, [Name], ModifiedDate AS ModifiedOn " +
                                     "FROM Production.ProductSubcategory " +
                                     "WHERE ProductSubcategoryId = @SubCategoryId";
                return connection.Query<SubCategory>(query, new { SubCategoryId = subCategoryId }).SingleOrDefault();
            }
        }

        public int InsertSubCategory(SubCategory subCategory)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "INSERT INTO Production.ProductSubcategory(ProductCategoryID, [Name]) " +
                                     "VALUES (@CategoryId, @Name)";
                int rowsAffectd = connection.Execute(query, subCategory);
                SetIdentity<int>(connection, id => subCategory.Id = id);
                return rowsAffectd;
            }
        }

        public int UpdateSubCategory(SubCategory subCategory)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "UPDATE Production.ProductSubcategory " +
                                     "SET ProductCategoryID = @CategoryId, " +
                                     "[Name] = @Name, " +
                                     "ModifiedDate = @ModifiedOn " +
                                     "WHERE ProductSubcategoryID = @Id";
                return connection.Execute(query, subCategory);
            }
        }

        public int DeleteSubCategory(SubCategory subCategory)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "DELETE FROM Production.ProductSubcategory " +
                                     "WHERE ProductSubcategoryID = @Id";
                return connection.Execute(query, subCategory);
            }
        }

        public IEnumerable<Model> SelectModels()
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT ProductModelID AS Id, [Name], CatalogDescription, Instructions, ModifiedDate AS ModifiedOn " +
                                     "FROM Production.ProductModel";
                return connection.Query<Model>(query);
            }
        }
    }
}