using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platz.ObjectBuilder.Schema;

namespace Platz.ObjectBuilder.Blazor.Controllers.Validation.Rules
{
    public class SelectFieldsUnique : IObjectBuilderRule
    {
        public RuleValidationResult Validate(IQueryController qc)
        {
            var fields = qc.SelectionProperties;

            var q = from f in fields
                    group f by f.OutputName into fg
                    where fg.Count() > 1
                    select fg.Key;

            var notUnique = q.ToList();

            if (notUnique.Any())
            {
                var result = new RuleValidationResult(GetType().Name, $"Query output fields must have unique names, these fields are not unqiue: {string.Join(", ", notUnique)}");
                return result;
            }

            return null;
        }
    }
}
