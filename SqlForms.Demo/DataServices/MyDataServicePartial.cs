using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Default
{
    public partial class MyDataService
    {
        public List<CustAddrCount> GetCustAddrCountListLike(QueryOptions options, string col, string like)
        {
            using (var db = GetDbContext())
            {
                var query = GetCustAddrCountListQuery(db, options, null);
                query = WhereLike(query, col, like);
                var result = query.ToList();
                return result;
            }
        }
    }
}
