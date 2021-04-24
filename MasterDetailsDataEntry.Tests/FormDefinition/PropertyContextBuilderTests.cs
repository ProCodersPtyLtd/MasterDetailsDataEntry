using Platz.SqlForms.Shared;
using SqlForms.Demo.MyForms.CrmBo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MasterDetailsDataEntry.Tests.FormDefinition
{
    public class PropertyContextBuilderTests
    {
        [Fact]
        public void DefaultContextMethodsTest()
        {
            var form = new ClientEditForm() as IDynamicEditForm;
            var fields = form.GetFields();
        }
    }
}
