using System;
using System.Collections.Generic;

namespace MasterDetailsDataEntry.Demo.Database.Model
{
    public partial class Client
    {
        public Client()
        {
            Order = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
