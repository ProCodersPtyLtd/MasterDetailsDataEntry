using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder
{
    public interface IStoreSchemaStorage
    {
        void SaveQuery(StoreQuery query, StorageParameters parameters);
        public void SaveSchema(StoreSchema schema, StorageParameters parameters);
    }

    public class StorageParameters
    {
        public string FileName { get; set; }
    }
}
