using System;
using System.Collections.Generic;

namespace MasterDetailsDataEntry.Demo.Database.Model
{
    public partial class Order
    {
        public Order()
        {
            OrderItem = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ExecuteDate { get; set; }

        public virtual ICollection<OrderItem> OrderItem { get; set; }
    }
}
