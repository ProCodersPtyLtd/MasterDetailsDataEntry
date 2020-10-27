using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Tests.FormDefinition
{
    public class TestModelsContext : DbContext
    {
    }

    public class TestOrder
    {
        public TestOrder()
        {
            OrderItem = new HashSet<TestOrderItem>();
        }

        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ExecuteDate { get; set; }
        public int ClientId { get; set; }

        public virtual TestClient Client { get; set; }
        public virtual ICollection<TestOrderItem> OrderItem { get; set; }
    }

    public partial class TestClient
    {
        public TestClient()
        {
            Order = new HashSet<TestOrder>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TestOrder> Order { get; set; }
    }

    public partial class TestOrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public bool? IsMajor { get; set; }
        public decimal Price { get; set; }

        public virtual TestOrder Order { get; set; }
    }
}
