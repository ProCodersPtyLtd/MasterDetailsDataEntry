using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Interfaces
{
    public interface IQueryGenerator
    {
        string GenerateQuery(string schemaFileName, string queryFileName);
    }
}
