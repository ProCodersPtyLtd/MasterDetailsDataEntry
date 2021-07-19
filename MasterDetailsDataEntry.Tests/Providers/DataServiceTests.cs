using Default;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MasterDetailsDataEntry.Tests.Providers
{
    public class DataServiceTests
    {
        [Fact]
        public void WhereLikeTest()
        {
            var service = new MyDataService();
            
            var options = new QueryOptions 
            { 
            };

            var data = service.GetCustAddrCountListLike(options, "FirstName", "%ja%");
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public void PagingTest()
        {
            var service = new MyDataService();

            var options = new QueryOptions
            {
                ApplyPagination = true,
                PageSize = 5
            };

            var data = service.GetCustAddrCountList(options);

            Assert.Equal(5, data.Count);
            Assert.True(options.PageReturnTotalCount > 0);
        }
    }
}
