using Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests
{
    public class SqlJsonPerformanceTests
    {
        private string[] _suburbs = new string[] { "gymea", "cronulla", "manli", "caringbah", "gosford", "woy woy", "hurtsvile" };
        private string[] _cities = new string[] { "sydney", "central coast", "melbourn", "gold coast", "brisban", "perth", "nowra" };
        private string[] _roads = new string[] { "wellingtone", "oak", "princess highway", "ewos pde", "sunset ave", "perth", "nowra" };
        private string[] _postcodes = new string[] { "1111", "2222", "2233", "3322", "2000", "2180", "1267" };

        private List<Address> _addresses = new List<Address>();

        public SqlJsonPerformanceTests()
        {             
            for (int i = 0; i < 10000; i++)
            {
                int x1 = i % 5;
                int x2 = i % 4;
                int x3 = i % 6 + 1;
                var a = new Address { Line1 = $"{i} {_roads[x1]}", City = _cities[x2], Suburb = _suburbs[x3], PostCode = _postcodes[x1] };
                _addresses.Add(a);
            }
        }

        //[Fact]
        public void Test1()
        {
            var db = new CrmDataContext();

            foreach(var a in _addresses)
            {
                db.Insert(a);
            }
        }

        [Fact]
        public void Test2()
        {
            var db = new CrmDataContext();

            var data = db.Get(typeof(Address));
        }

        [Fact]
        public void Test3()
        {
            var db = new CrmDataContext();

            for(int i = 1; i < 10001; i++)
            {
                var data = db.Find(typeof(Address), i);
            }
        }
    }
}
