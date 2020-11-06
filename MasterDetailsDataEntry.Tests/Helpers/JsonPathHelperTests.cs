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
    }
}
