using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.Services
{
    public class FileAccessService : IFileAccess
    {
        public bool CheckLocalFileExist(string filename)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CopyFromAssetsToLocal(string destFilename, string assetFilename)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CopyToLocal(string sourceFile, string destFilename)
        {
            throw new NotImplementedException();
        }

        public string GetLocalFilePath(string filename)
        {
            var appPath = FileSystem.Current.AppDataDirectory;
            return Path.Combine(appPath, filename); 
        }

        public Task<string> ReadAssetFile(string filename)
        {
            throw new NotImplementedException();
        }

        public Task WriteToFile(byte[] data, string filename)
        {
            throw new NotImplementedException();
        }
    }
}
