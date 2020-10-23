using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MasterDetailsDataEntry.Shared.Controls;
using MasterDetailsDataEntry.Shared.Helpers;

namespace MasterDetailsDataEntry.Shared.Forms
{
    public abstract class MasterDetailsForm<M, D> : IMultiModelDefinitionForm
        where M : class
        where D : class
    {
        // private string _pkField;
        // private string _foreignKeyField;
        private Dictionary<string, DataField> _fields;
        private Dictionary<string, DataField> _masterFields;

        protected abstract void Define();

        public MasterDetailsForm<M, D> PrimaryKey<TKey>(Expression<Func<M, TKey>> selector) 
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();

            if (!_masterFields.ContainsKey(bindingProperty))
            {
                _masterFields[bindingProperty] = new DataField();
            }

            _masterFields[bindingProperty].BindingProperty = bindingProperty;
            _masterFields[bindingProperty].PrimaryKey = true;
            return this;
        }

        public MasterDetailsForm<M, D> ForeignKey<TKey>(Expression<Func<D, TKey>> selector)
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();

            if (!_fields.ContainsKey(bindingProperty))
            {
                _fields[bindingProperty] = new DataField();
            }

            _fields[bindingProperty].BindingProperty = bindingProperty;
            _fields[bindingProperty].ForeignKey = true;
            return this;
        }

        public MasterDetailsForm<M, D> Field<TKey>(Expression<Func<D, TKey>> selector, Field field)
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();

            if (!_fields.ContainsKey(bindingProperty))
            {
                _fields[bindingProperty] = new DataField();
            }

            _fields[bindingProperty].CopyFrom(field);
            _fields[bindingProperty].BindingProperty = bindingProperty;
            return this;
        }

        // Use Include or Exclude approach, don't use both
        public MasterDetailsForm<M, D> IncludeField<TKey>(Expression<Func<D, TKey>> selector)
        {
            return this;
        }

        public MasterDetailsForm<M, D> ExcludeField<TKey>(Expression<Func<D, TKey>> selector)
        {
            return this;
        }

        public MasterDetailsForm<M, D> MasterField<TKey>(Expression<Func<M, TKey>> selector, Field field)
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();

            if (!_masterFields.ContainsKey(bindingProperty))
            {
                _masterFields[bindingProperty] = new DataField();
            }

            _masterFields[bindingProperty].CopyFrom(field);
            _masterFields[bindingProperty].BindingProperty = bindingProperty;
            return this;
        }

        public Tuple<List<DataField>, List<DataField>> GetFields()
        {
            _fields = new Dictionary<string, DataField>();
            _masterFields = new Dictionary<string, DataField>();
            Define();

            return new Tuple<List<DataField>, List<DataField>>(
                PrepareFields(_masterFields.Values.ToList(), typeof(M)), 
                PrepareFields(_fields.Values.ToList(), typeof(D)));
        }

        public static List<DataField> PrepareFields(List<DataField> list, Type type)
        {
            foreach (var field in list)
            {
                var property = type.GetProperty(field.BindingProperty);
                field.DataType = property.PropertyType;

                if (field.ControlType == null)
                {
                    var dataTypeName = field.DataType.Name;

                    if (field.DataType.Name == "Nullable`1")
                    {
                        dataTypeName = Nullable.GetUnderlyingType(field.DataType).Name;
                    }

                    switch (dataTypeName)
                    {
                        case "Boolean":
                            field.ControlType = typeof(DefaultCheckboxControl);
                            break;
                        case "DateTime":
                            field.ControlType = typeof(DefaultDateEditControl);
                            break;
                        default:
                            field.ControlType = typeof(DefaultTextEditControl);
                            break;
                    }
                }
            }

            return list;
        }
    }

    public class MyForm : MasterDetailsForm<A, B>
    {
        protected override void Define()
        {
            this
                .PrimaryKey(m => m.Id)
                .ForeignKey(d => d.MasterId)
                .MasterField(m => m.Id, new Field { Hidden = true })
                .MasterField(m => m.OrderDate, new Field { Required = true })
                .Field(d => d.MasterId, new Field { Hidden = true })
                ;
        }
    }

    public class A
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class B
    {
        public int MasterId { get; set; }
    }
}
