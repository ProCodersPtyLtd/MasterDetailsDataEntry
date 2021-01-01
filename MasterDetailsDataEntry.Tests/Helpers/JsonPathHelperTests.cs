using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Platz.SqlForms.Shared;
using System.Linq;

namespace MasterDetailsDataEntry.Tests.Helpers
{
    public class JsonPathHelperTests
    {
        [Fact]
        public void ToSqlStringTest()
        {
            object value = "value";
            Assert.Equal("\"value\"", value.ToSql());
        }

        [Fact]
        public void FormatTest()
        {
            var contextName = "DemoSqlForms.Database.Model.SchoolContext";
            var contextNamelist = contextName.Split('.').ToList();
            var ctxName = contextNamelist.Last();
            contextNamelist.Remove(contextNamelist.Last());
            var namespaceName = string.Join(".", contextNamelist.ToArray());

            var p = string.Format("CustAddrList/{1}", 10, 20);
            Assert.Equal("CustAddrList/20", p);

            p = string.Format("CustAddrList/{1}", 10, new object[] { 20 });
            Assert.NotEqual("CustAddrList/20", p);

            p = string.Format("CustAddrList/{1}", new object[] { 10, 20 });
            Assert.Equal("CustAddrList/20", p);
        }
    }
}
