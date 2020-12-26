//using Platz.SqlForms.Shared;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;

//namespace Platz.SqlForms
//{
//    public class DynamicFormEntityTypeBuilder<TEntity> : FormEntityTypeBuilder<TEntity> where TEntity : class
//    {
//        public DynamicFormEntityTypeBuilder()
//        {
//        }

//        public virtual FormEntityTypeBuilder<TEntity> DialogButton(ButtonActionTypes actionType, string buttonText = null, string hint = null)
//        {
//            DialogButtons.Add(new DialogButtonDetails { Action = actionType, Text = buttonText, Hint = hint });
//            return this;
//        }

//    }
//}
