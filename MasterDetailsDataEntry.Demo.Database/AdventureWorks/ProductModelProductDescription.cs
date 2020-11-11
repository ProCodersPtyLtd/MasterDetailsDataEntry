using System;
using System.Collections.Generic;

namespace MasterDetailsDataEntry.Demo.Database.AdventureWorks
{
    public partial class ProductModelProductDescription
    {
        public int ProductModelId { get; set; }
        public int ProductDescriptionId { get; set; }
        public string Culture { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ProductDescription ProductDescription { get; set; }
        public virtual ProductModel ProductModel { get; set; }
    }
}
