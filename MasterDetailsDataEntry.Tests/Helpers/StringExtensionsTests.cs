using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MasterDetailsDataEntry.Tests.Helpers
{
    public class StringExtensionsTests
    {
        [Fact]
        public void TrimNameTest()
        {
            Assert.Equal("Name1", "34Name1".TrimName());
            Assert.Equal("Name1", "% Name 1".TrimName());
            Assert.Equal("MyName1", "M%y Name ^^ 1".TrimName());
            Assert.Equal("MyLabel", "% My Label %".TrimName());
            Assert.Equal("MyLabel", " My Label            ".TrimName());
            Assert.Equal("MyLabel2", "!! My! !Label#@$&^%()$#&*% 2#@".TrimName());
            Assert.Equal("_MyLabel", "_!! My! !Label ".TrimName());
        }
    }
}
