using System;
using System.Collections.Generic;

namespace SqlForms.Demo.Database.AdventureWorks
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            InverseParentProductCategory = new HashSet<ProductCategory>();
            Product = new HashSet<Product>();
        }

        public int ProductCategoryId { get; set; }
        public int? ParentProductCategoryId { get; set; }
        public string Name { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ProductCategory ParentProductCategory { get; set; }
        public virtual ICollection<ProductCategory> InverseParentProductCategory { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
