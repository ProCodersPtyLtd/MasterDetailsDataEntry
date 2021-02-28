using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Default
{
    public partial class CrmDataContext
    {
        public CrmDataContext() : base(null, "CrmConnection")
        {
        }
    }
}
