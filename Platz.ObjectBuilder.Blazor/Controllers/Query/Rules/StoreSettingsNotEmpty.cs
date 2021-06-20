using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public class StoreSettingsNotEmpty : IQueryBuilderRule
    {
        public RuleValidationResult Validate(IQueryControllerModel qm)
        {
            var list = new List<RuleValidationResult>();

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.DataService))
            {
                list.Add(new RuleValidationResult($"Settings Parameter 'DataService' cannot be empty"));
            }

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.Namespace))
            {
                list.Add(new RuleValidationResult($"Settings Parameter 'Namespace' cannot be empty"));
            }

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.QueryName))
            {
                list.Add(new RuleValidationResult($"Settings Parameter 'QueryName' cannot be empty"));
            }

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.QueryReturnType))
            {
                list.Add(new RuleValidationResult($"Settings Parameter 'QueryReturnType' cannot be empty"));
            }

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.StoreDataPath))
            {
                list.Add(new RuleValidationResult($"Settings Parameter 'StoreDataPath' cannot be empty"));
            }

            if (list.Any())
            {
                var result = new RuleValidationResult($"StoreSettingsNotEmpty failed") { Results = list };
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
