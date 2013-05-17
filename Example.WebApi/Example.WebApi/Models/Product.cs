using System.Runtime.InteropServices;

namespace TestBase.Example.WebApi.Models
{
    public class Product
    {
        public Product([Optional] string Name, [Optional] int Id, [Optional] string Category, [Optional] decimal Price)
        {
            this.Price = Price;
            this.Category = Category;
            this.Id = Id;
            this.Name = Name;
        }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Category { get; private set; }
        public decimal Price { get; private set; }
    }
}