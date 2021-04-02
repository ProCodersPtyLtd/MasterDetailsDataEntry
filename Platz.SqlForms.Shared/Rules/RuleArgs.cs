using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public abstract class RuleArgs
    {
        public abstract FormEntityTypeBuilder EntityBuilder { get; }
    }

    public class RuleArgs<TEntity>: RuleArgs where TEntity: class
    {
        public RuleArgs(TEntity model)
        {
            M = model;
            E = new FormEntityTypeBuilder<TEntity>();
        }

        public TEntity M { get; private set; }
        public FormEntityTypeBuilder<TEntity> E { get; private set; }

        public TEntity Model {  get { return M; } }
        public FormEntityTypeBuilder<TEntity> Entity {  get { return E; } }

        public override FormEntityTypeBuilder EntityBuilder { get { return E; } }
    }
}
