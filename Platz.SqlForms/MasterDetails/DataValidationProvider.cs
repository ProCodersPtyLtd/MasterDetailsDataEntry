using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public class DataValidationProvider : IDataValidationProvider
    {
        private readonly IDataEntryProvider _dataEntryProvider;

        public DataValidationProvider(IDataEntryProvider dataEntryProvider)
        {
            _dataEntryProvider = dataEntryProvider;
        }

        public IEnumerable<ValidationResult> ValidateCustomRules(IDataForm form, object item, int rowIndex, IEnumerable<DataField> fields, FormRuleTriggers trigger)
        {
            var result = new List<ValidationResult>();

            foreach (var field in fields)
            {
                CustomRule(form, fields, item, rowIndex, field, result, trigger);
            }

            return result;
        }

        public IEnumerable<ValidationResult> ValidateModel(IDataForm form, object item, int rowIndex, IEnumerable<DataField> fields, Dictionary<string, FieldState> states = null)
        {
            var result = new List<ValidationResult>();

            foreach (var field in fields)
            {
                PrimaryKeyRule(item, rowIndex, field, result);
                RequiredRule(item, rowIndex, field, result);

                // Ignore this rule for Update by PrimaryKey
                if (states == null || !field.PrimaryKey || states[field.BindingProperty].IsNew)
                {
                    UniqueRule(form, item, rowIndex, field, result);
                }
                
                CustomRule(form, fields, item, rowIndex, field, result, FormRuleTriggers.Submit);
            }

            return result;
        }

        public IEnumerable<ValidationResult> ValidateModelProperty(IDataForm form, object item, int rowIndex, string bindingProperty, IEnumerable<DataField> fields)
        {
            var result = new List<ValidationResult>();
            var field = fields.Single(f => f.BindingProperty == bindingProperty);

            PrimaryKeyRule(item, rowIndex, field, result);
            RequiredRule(item, rowIndex, field, result);
            UniqueRule(form, item, rowIndex, field, result);
            CustomRule(form, fields, item, rowIndex, field, result, FormRuleTriggers.Change);
            
            return result;
        }

        private void CustomRule(IDataForm form, IEnumerable<DataField> fields, object item, int rowIndex, DataField field, List<ValidationResult> result, 
            FormRuleTriggers trigger)
        {
            var rules = field.Rules.Where(r => r.Trigger == trigger 
                || (trigger == FormRuleTriggers.ChangeSubmit && (r.Trigger == FormRuleTriggers.Change || r.Trigger == FormRuleTriggers.Submit))
                || (trigger == FormRuleTriggers.Submit && (r.Trigger == FormRuleTriggers.Change || r.Trigger == FormRuleTriggers.Submit || r.Trigger == FormRuleTriggers.ChangeSubmit))
                || (trigger == FormRuleTriggers.Change && (r.Trigger == FormRuleTriggers.Change || r.Trigger == FormRuleTriggers.Submit || r.Trigger == FormRuleTriggers.ChangeSubmit))
                );

            foreach (var rule in rules)
            {
                FormRuleResult vr;

                if (rule.Method != null)
                {
                    vr = rule.Method(item);
                }
                else
                {
                    rule.EntityBuilder = Activator.CreateInstance(rule.BuilderType) as FormEntityTypeBuilder;
                    vr = rule.MethodB(item, rule.EntityBuilder);
                    var ruleFields = rule.EntityBuilder.GetFieldDictionary();
                    
                    //ToDo: update fields appearance here
                    foreach (var f in fields)
                    {
                        var rf = ruleFields[f.BindingProperty];
                        f.Required = rf.Required;
                        f.Hidden = rf.Hidden;
                        f.ReadOnly = rf.ReadOnly;
                        f.Label = rf.Label;
                    }
                }

                if (vr != null && vr.IsFailed)
                {
                    result.Add(new ValidationResult
                    {
                        Message = $"{vr.RuleName}: {vr.Message}",
                        BindingProperty = field.BindingProperty,
                        ValidationResultType = ValidationResultTypes.Error,
                        RowIndex = rowIndex
                    });
                }
            }
        }

        private void RequiredRule(object item, int rowIndex, DataField field, List<ValidationResult> result)
        {
            if (field.Required)
            {
                var value = item.GetPropertyValue(field.BindingProperty);

                if (value == null || value.ToString() == "" || (field.SelectEntityType != null && value.ToString() == "0"))
                {
                    result.Add(new ValidationResult
                    {
                        Message = "Required",
                        BindingProperty = field.BindingProperty,
                        ValidationResultType = ValidationResultTypes.Error,
                        RowIndex = rowIndex
                    });
                }
            }
        }
        private void PrimaryKeyRule(object item, int rowIndex, DataField field, List<ValidationResult> result)
        {
            var value = item.GetPropertyValue(field.BindingProperty);

            if (field.PrimaryKey && field.PrimaryKeyGeneratedType != PrimaryKeyGeneratedTypes.OnAdd && (value is int) && (int)value == 0)
            {
                result.Add(new ValidationResult
                {
                    Message = "PK cannot be 0",
                    BindingProperty = field.BindingProperty,
                    ValidationResultType = ValidationResultTypes.Error,
                    RowIndex = rowIndex
                });
            }
        }

        private void UniqueRule(IDataForm form, object item, int rowIndex, DataField field, List<ValidationResult> result)
        {
            if (field.Unique)
            {
                if (_dataEntryProvider.IsPropertyValueNotUnique(form, item, field.BindingProperty, item.GetType()))
                {
                    result.Add(new ValidationResult
                    {
                        Message = "Not unique",
                        BindingProperty = field.BindingProperty,
                        ValidationResultType = ValidationResultTypes.Error,
                        RowIndex = rowIndex
                    });
                }
            }
        }
    }
}
 