﻿using Platz.SqlForms;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Model
{
    public class FieldComponentModel
    {
        public FieldComponentModel()
        { 
        }

        public FieldComponentModel(StoreFormField f)
        {
            CopyFrom(this, f);
        }

        public FieldComponentModel(StoreFormButton b)
        {
            CopyFrom(this, b);
        }

        //public string Name { get; set; }
        //public string Binding { get; set; }
        public bool Active { get; set; }
        public bool FullView { get; set; } 
        public int Order { get; set; }
        public FieldComponentType ComponentType { get; set; }

        public StoreFormField StoreField { get; set; } = new StoreFormField();
        public StoreFormButton StoreButton { get; set; } = new StoreFormButton();

        public List<FieldRuleModel> Rules { get; set; } = new List<FieldRuleModel>();

        public static void CopyFrom(FieldComponentModel model, StoreFormField field)
        {
            model.StoreField = field.GetCopy();
            model.Order = field.Order;
            model.ComponentType = GetComponentType(field.ControlType);
        }

        public static void CopyFrom(FieldComponentModel model, StoreFormButton button)
        {
            model.StoreButton = button.GetCopy();
            model.Order = button.Order;
            model.ComponentType = FieldComponentType.ActionButton;
            model.StoreButton.NavigationParameterMapping = new List<StoreNavigationParameter>();
            button.NavigationParameterMapping.CopyListTo(model.StoreButton.NavigationParameterMapping);
        }

        public StoreFormField ToStore()
        {
            var field = StoreField.GetCopy();
            field.ControlType = ComponentType.ToString();

            return field;
        }

        public StoreFormButton ToStoreButton()
        {
            var button = StoreButton.GetCopy();
            button.Order = Order;
            // prepare new collection as target for copy operation
            button.NavigationParameterMapping = new List<StoreNavigationParameter>();
            StoreButton.NavigationParameterMapping.CopyListTo(button.NavigationParameterMapping);

            return button;
        }

        private static Dictionary<string, FieldComponentType> FieldComponentTypeMap = new Dictionary<string, FieldComponentType> 
        {
            { "TextEdit", FieldComponentType.TextEdit },
            { "DateEdit", FieldComponentType.DateEdit },
            { "NumberEdit", FieldComponentType.NumberEdit },
            { "Checkbox", FieldComponentType.Checkbox },
            { "Dropdown", FieldComponentType.Dropdown },
            { "ActionButton", FieldComponentType.ActionButton },
        };

        private static FieldComponentType GetComponentType(string controlType)
        {
            return FieldComponentTypeMap[controlType];
        }
    }

    public enum FieldComponentType
    {
        TextEdit,
        DateEdit,
        NumberEdit,
        Checkbox,
        Dropdown,
        ActionButton
    }
}
