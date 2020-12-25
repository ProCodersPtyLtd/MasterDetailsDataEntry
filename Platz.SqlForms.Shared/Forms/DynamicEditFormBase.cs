using Microsoft.EntityFrameworkCore;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public abstract class DynamicEditFormBase
    {
        protected readonly IDataFieldProcessor _dataFieldProcessor;
        protected DynamicFormBuilder _builder;

        public DynamicEditFormBase()
        {
            _dataFieldProcessor = new DefaultDataFieldProcessor();
            _builder = new DynamicFormBuilder();
            Define(_builder);
        }

        protected virtual void Define(DynamicFormBuilder builder)
        {
        }
    }

    public abstract class DynamicEditFormBase<T> : DynamicEditFormBase where T : DbContext
    {
        protected T GetDbContext()
        {
            var context = Activator.CreateInstance<T>();
            return context;
        }
    }
}
