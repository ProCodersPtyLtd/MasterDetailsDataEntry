using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platz.ObjectBuilder.Schema;
using Platz.SqlForms;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public class JoinCompatibleTypeRule : SubQueryBuilderRuleBase
    {
        public override RuleValidationResult Validate(IQueryModel qm)
        {
            var badJoins = new List<string>();
            var joins = qm.FromTableJoins.Where(j => !j.IsDeleted);

            foreach (var join in joins)
            {
                var leftObject = qm.FromTables.First(t => t.Alias == join.Source.LeftObjectAlias);
                var leftProp = leftObject.Properties.First(p => p.Name == join.Source.LeftField).OriginalStoreProperty;
                var rightObject = qm.FromTables.First(t => t.Alias == join.Source.RightObjectAlias);
                var rightProp = rightObject.Properties.First(p => p.Name == join.Source.RightField).OriginalStoreProperty;

                if (GetPropertyDataType(leftProp) != GetPropertyDataType(rightProp))
                {
                    badJoins.Add($"{join.Source.GetJoinString()}({leftProp.Type} = {rightProp.Type})");
                }
            }

            if (badJoins.Any())
            {
                var result = new RuleValidationResult(
                        GetType().Name,
                        $"Query '{qm.Name}' has Joins that compare incompatible types: {string.Join(", ", badJoins)}");

                return result;
            }
            return null;
        }

        private string GetPropertyDataType(StoreProperty p)
        {
            if (p.Type == "reference" && p.Fk)
            {
                return p.ForeignKeys[0].Type;
            }

            return p.Type;
        }
    }
}
