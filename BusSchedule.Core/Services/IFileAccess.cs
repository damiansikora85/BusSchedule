using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.Services
{
    public interface IFileAccess
    {
        string GetLocalFilePath(string filename);
        Task<bool> CopyFromAssetsToLocal(string destFilename, string assetFilename);
        bool CheckLocalFileExist(string filename);
        Task<string> ReadAssetFile(string filename);
        Task WriteToFile(byte[] data, string filename);
        Task<bool> CopyToLocal(string sourceFile, string destFilename);
    }
}
