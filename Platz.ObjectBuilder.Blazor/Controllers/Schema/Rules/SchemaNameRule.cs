using Platz.ObjectBuilder.Blazor.Controllers.Validation;
using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Controllers.Schema.Rules
{
    public class SchemaNameRule : ISchemaBuilderRule
    {
        public RuleValidationResult Validate(DesignSchema model)
        {
            return TableNameRule.CheckObjectName("Schema", model.Name);
        }
    }
}
