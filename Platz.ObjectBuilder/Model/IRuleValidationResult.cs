using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public interface IRuleValidationResult
    {
        bool IsFailed { get; }
        List<IRuleValidationResult> Results { get; }
    }

    //public interface IRuleValidationResult<R>
    //    where R : IRuleValidationResult
    //{
    //    public List<R> Results { get; }
    //}
}
