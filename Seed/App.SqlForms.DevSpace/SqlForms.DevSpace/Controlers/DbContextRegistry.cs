using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace SqlForms.DevSpace.Controlers
{
    public interface IDbContextRegistry
    {
        IEnumerable<Type> GetContexts();
    }

    public class DbContextRegistry : IDbContextRegistry
    {
        public IEnumerable<Type> GetContexts()
        {
            return new Type[] { typeof(Demo.Database.AdventureWorks.AdventureWorksContext) };
        }
    }
}
