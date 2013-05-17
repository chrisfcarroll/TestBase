using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Models
{
    public class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CatalogDescription { get; set; }
        public string Instructions { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}