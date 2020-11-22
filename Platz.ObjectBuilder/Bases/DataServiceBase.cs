using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder
{
    public abstract class DataServiceBase<T> where T: DbContext
    {
        protected DbContext Context { get; set; }

        protected T GetDbContext()
        {
            return Context as T;
        }
    }
}
