using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public interface IProjectLoader
    {
        StoreProject Load(string folderName);
        void SaveAll(StoreProject project, string location, List<ObjectRenameItem> renames);
        List<string> GetFolders(string location);

        StoreSchema LoadSchema(string folderName, string name);
        StoreQuery LoadQuery(string folderName, string name);
    }
}
