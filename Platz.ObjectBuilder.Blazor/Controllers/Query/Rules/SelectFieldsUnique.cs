using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platz.ObjectBuilder.Schema;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public class SelectFieldsUnique : IQueryBuilderRule
    {
        public RuleValidationResult Validate(IQueryModel qm)
        {
            var fields = qm.SelectionProperties;

            var q = from f in fields
                    where f.IsOutput
                    group f by f.OutputName into fg
                    where fg.Count() > 1
                    select fg.Key;

            var notUnique = q.ToList();

            if (notUnique.Any())
            {
                var result = new RuleValidationResult(GetType().Name, $"Query output fields must have unique names, these fields are not unique: {string.Join(", ", notUnique)}");
                return result;
            }

            return null;
        }
    }
}
