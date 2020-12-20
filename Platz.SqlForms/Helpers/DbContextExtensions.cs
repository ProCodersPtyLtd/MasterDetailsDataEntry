using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Platz.SqlForms
{
    public static class DbContextExtensions
    {
        public static IQueryable FindSet(this DbContext db, Type entityType)
        {
            MethodInfo method = typeof(DbContext).GetMethod("Set");
            MethodInfo generic = method.MakeGenericMethod(entityType);
            IQueryable queryable = ((IQueryable)generic.Invoke(db, null)).Cast(entityType);
            return queryable;
        }

        public static string FindSinglePrimaryKey(this DbContext db, Type entityType)
        {
            var entityMetadata = db.Model.GetEntityTypes().Single(p => p.ClrType == entityType);
            var pk = entityMetadata.FindPrimaryKey().Properties.First().Name;
            return pk;
        }

        public static IProperty FindSinglePrimaryKeyProperty(this DbContext db, Type entityType)
        {
            var entityMetadata = db.Model.GetEntityTypes().Single(p => p.ClrType == entityType);
            var pk = entityMetadata.FindPrimaryKey().Properties.First();
            return pk;
        }
    }
}
