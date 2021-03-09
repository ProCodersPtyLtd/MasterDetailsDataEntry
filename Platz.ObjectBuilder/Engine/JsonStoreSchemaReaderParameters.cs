using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Engine
{
    public class JsonStoreSchemaReaderParameters : IStoreSchemaReaderParameters
    {
        public string SchemaFile { get; set; }
    }
}
