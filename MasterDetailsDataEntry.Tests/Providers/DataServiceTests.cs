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
        public void WhereLiIkeTest()
        {
            var service = new MyDataService();
            
            var options = new QueryOptions 
            { 
            };

            var data = service.GetCustAddrCountListLike(options, "FirstName", "%ja%");
        }
    }
}
