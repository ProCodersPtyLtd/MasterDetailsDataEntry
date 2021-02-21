using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder
{
    public interface IStoreSchemaStorage
    {
        void SaveQuery(StoreQuery query, StorageParameters parameters);
        void SaveSchema(StoreSchema schema, StorageParameters parameters);
        List<string> GetFileNames(StorageParameters parameters);
        bool FileExists(StorageParameters parameters);
        StoreQuery LoadQuery(StorageParameters parameters);
        StoreSchema LoadSchema(StorageParameters parameters);
    }

    public class StorageParameters
    {
        public string Path { get; set; }
        public string FileName { get; set; }
    }
}
