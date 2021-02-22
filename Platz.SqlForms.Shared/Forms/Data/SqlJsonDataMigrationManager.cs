using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class SqlJsonDataMigrationManager : IDataMigrationManager
    {
        private readonly IStoreDatabaseDriver _storeDatabaseDriver;

        public SqlJsonDataMigrationManager(IStoreDatabaseDriver storeDatabaseDriver)
        {
            _storeDatabaseDriver = storeDatabaseDriver;
        }

        //public void ApplyMigrations(StoreSchemaMigrations package)
        //{ 
        //}
    }
}
