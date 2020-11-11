using System;
using System.Collections.Generic;

namespace MasterDetailsDataEntry.Demo.Database.AdventureWorks
{
    public partial class VGetAllCategories
    {
        public string ParentProductCategoryName { get; set; }
        public string ProductCategoryName { get; set; }
        public int? ProductCategoryId { get; set; }
    }
}
