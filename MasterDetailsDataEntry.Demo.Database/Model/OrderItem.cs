using System;
using System.Collections.Generic;

namespace MasterDetailsDataEntry.Demo.Database.Model
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public bool? IsMajor { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; }
    }
}
