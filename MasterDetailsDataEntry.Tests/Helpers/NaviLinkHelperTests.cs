using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MasterDetailsDataEntry.Tests.Helpers
{
    public class NaviLinkHelperTests
    {
        [Fact]
        public void IntsTest()
        {
            var ps = new object[] { 1, 2 };
            var f = @"link\{0}\{1}";
            var result = NaviLinkHelper.GetLink(f, ps);
            Assert.Equal(@"link\1\2", result);
        }

        [Fact]
        public void IntsNegativeTest()
        {
            var ps = new object[] { 1, 2 };
            var f = @"link\{0}\{1}\{2}";
            var ex = Assert.Throws<NavigationLinkException>(() => NaviLinkHelper.GetLink(f, ps));
        }

        [Fact]
        public void FormParametersTest()
        {
            var ps = new FormParameter[]
            {
                new FormParameter("AddressId", 10),
                new FormParameter("CustomerId", 20),
            };
            var f = @"link\{AddressId}\{CustomerId}";
            var result = NaviLinkHelper.GetLink(f, ps);
            Assert.Equal(@"link\10\20", result);
        }

        [Fact]
        public void FormParametersSortedTest()
        {
            var ps = new FormParameter[]
            {
                new FormParameter("AddressId", 10),
                new FormParameter("CustomerId", 20),
            };
            var f = @"link\{CustomerId}\{AddressId}";
            var result = NaviLinkHelper.GetLink(f, ps);
            Assert.Equal(@"link\20\10", result);
        }

        [Fact]
        public void FormParametersNegativeTest()
        {
            var ps = new FormParameter[]
            {
                new FormParameter("AddressId", 10),
                new FormParameter("CustomerId", 20),
            };
            var f = @"link\{0}\{1}";
            var result = NaviLinkHelper.GetLink(f, ps);
            Assert.Equal(@"link\{0}\{1}", result);
        }
    }
}
