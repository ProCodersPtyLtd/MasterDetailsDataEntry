using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Validation
{
    public interface IBuilderRule<M>
    {
        RuleValidationResult Validate(M model);
    }

    public interface IBuilderRuleFactory<T, M> where T: IBuilderRule<M>
    {
        List<T> GetAllRules();
        void RegisterRule(T rule);
        List<RuleValidationResult> ValidateAllRules(M qm);
    }

    public class BuilderRuleFactory<T, M> : IBuilderRuleFactory<T, M> where T : IBuilderRule<M>
    {
        private readonly List<T> _rules;

        public BuilderRuleFactory()
        {
            _rules = new List<T>();
            var localRules = GetLocalRules();
            _rules.AddRange(localRules);
        }

        public List<T> GetAllRules()
        {
            return _rules;
        }

        public void RegisterRule(T rule)
        {
            if (!_rules.Any(r => r.GetType() == rule.GetType()))
            {
                _rules.Add(rule);
            }
        }

        public List<RuleValidationResult> ValidateAllRules(M qm)
        {
            var result = new List<RuleValidationResult>();
            var rules = GetAllRules();

            foreach (var rule in rules)
            {
                var vr = rule.Validate(qm);

                if (vr != null && vr.IsFailed)
                {
                    result.Add(vr);
                }
            }

            return result;
        }

        private List<T> GetLocalRules()
        {
            var result = typeof(BuilderRuleFactory<,>).Assembly.GetTypes().Where(t => !t.IsInterface && typeof(T).IsAssignableFrom(t))
                .Select(t => (T)Activator.CreateInstance(t))
                .ToList();

            return result;
        }
    }
}
