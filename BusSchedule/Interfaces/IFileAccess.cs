using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Interfaces
{
    public interface IFileAccess
    {
        string GetLocalFilePath(string filename);
        Task<bool> CopyFromAssetsToLocal(string destFilename, string assetFilename);
        bool CheckLocalFileExist(string v);
        Task<string> ReadAssetFile(string v);
    }
}
