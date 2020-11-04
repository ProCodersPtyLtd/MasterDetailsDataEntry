using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Platz.SqlForms
{
    public class FormBuilder
    {
        private List<FormEntityTypeBuilder> _builders = new List<FormEntityTypeBuilder>();
        private List<Type> _entities = new List<Type>();
        private int _masterEntityIndex = -1;

        public virtual FormBuilder Entity<TEntity>([NotNullAttribute] Action<FormEntityTypeBuilder<TEntity>> buildAction) where TEntity : class
        {
            var builder = new FormEntityTypeBuilder<TEntity>();
            buildAction.Invoke(builder);
            _builders.Add(builder);
            _entities.Add(typeof(TEntity));
            return this;
        }

        public virtual FormBuilder IsMaster()
        {
            _masterEntityIndex = _entities.Count - 1;
            return this;
        }

        public List<FormEntityTypeBuilder> Builders { get { return _builders; } }
        public List<Type> Entities { get { return _entities; } }
        public int MasterEntityIndex { get { return _masterEntityIndex; } }
    }
}
