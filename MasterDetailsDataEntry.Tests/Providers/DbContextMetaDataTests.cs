using MasterDetailsDataEntry.Demo.Database.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace MasterDetailsDataEntry.Tests.Providers
{
    public class DbContextMetaDataTests
    {
        [Fact]
        public void ReadEntityMetadataTest()
        {
            using (var db = new TestContext())
            {
                foreach (var entityType in db.Model.GetEntityTypes())
                {
                    var tableName = entityType.GetTableName();
                    foreach (var propertyType in entityType.GetProperties())
                    {
                        var columnName = propertyType.GetColumnName();
                    }
                }
            }
        }
    }
}
