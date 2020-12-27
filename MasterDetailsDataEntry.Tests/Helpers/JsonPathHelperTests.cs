using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Platz.SqlForms.Shared;

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
            var p = string.Format("CustAddrList/{1}", 10, 20);
            Assert.Equal("CustAddrList/20", p);

            p = string.Format("CustAddrList/{1}", 10, new object[] { 20 });
            Assert.NotEqual("CustAddrList/20", p);

            p = string.Format("CustAddrList/{1}", new object[] { 10, 20 });
            Assert.Equal("CustAddrList/20", p);
        }
    }
}
