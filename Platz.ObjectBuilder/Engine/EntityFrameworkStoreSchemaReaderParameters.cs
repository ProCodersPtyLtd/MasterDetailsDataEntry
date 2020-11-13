using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Engine
{
    public class EntityFrameworkStoreSchemaReaderParameters : IStoreSchemaReaderParameters
    {
        public Type DbContextType { get; set; }
    }
}
