using Platz.SqlForms;
using SqlForms.DevSpace.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.DevSpace.Logic
{
    public interface IProjectLoader
    {
        StoreProject Load(string folderName);
        void SaveAll(StoreProject project, string location, List<ObjectRenameItem> renames);
        List<string> GetFolders(string location);
    }
}
