using Platz.ObjectBuilder.Blazor.Model;
using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Controllers.Schema.Rules
{
    public class FormNameRule : IFormBuilderRule
    {
        public RuleValidationResult Validate(FormBuilderModel model)
        {
            var result = TableNameRule.CheckObjectName("Form", model.Name);
            return result;
        }
    }
}
