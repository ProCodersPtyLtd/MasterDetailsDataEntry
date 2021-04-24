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
    }
}
