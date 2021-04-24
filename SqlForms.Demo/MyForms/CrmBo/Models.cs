using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrmServices;

namespace SqlForms.Demo.MyForms.CrmBo
{
    public class ClientModel
    {
        public Person Client { get; set; }
        public Contact ClientContact { get; set; }
        public List<Address> ClientAddressList { get; set; }
    }
}
