using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Default
{
    public partial class CrmDataContext
    {
        public CrmDataContext() : base(new DataContextParams { ConnectionStringConfigKey = "CrmConnection" })
        {
        }
    }
}
