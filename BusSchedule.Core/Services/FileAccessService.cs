using Microsoft.Maui.Controls.PlatformConfiguration;
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
            var localFilename = GetLocalFilePath(filename);
            return File.Exists(localFilename);
        }

        public Task<bool> CopyFromAssetsToLocal(string destFilename, string assetFilename)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task.Run(async () =>
            {
                try
                {
                    if (File.Exists(destFilename))
                    {
                        File.Delete(destFilename);
                    }
                    using var br = new BinaryReader(await FileSystem.Current.OpenAppPackageFileAsync(assetFilename));
                    using var bw = new BinaryWriter(new FileStream(destFilename, FileMode.Create));
                    byte[] buffer = new byte[2048];
                    int length = 0;
                    while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, length);
                    }
                }
                catch (Exception exc)
                {
                    var msg = exc.Message;
                }
                finally
                {
                    tcs.SetResult(true);
                }
            }).ConfigureAwait(false);

            return tcs.Task;
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

        public async Task<string> ReadAssetFile(string filename)
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(filename);
            using StreamReader reader = new(fileStream);
            try
            {
                var text = await reader.ReadToEndAsync();
            }
            catch(Exception exc)
            {
                var msg = exc.Message;
            }
            return await reader.ReadToEndAsync();
        }

        public Task WriteToFile(byte[] data, string filename)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task.Run(() =>
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    var stream = new MemoryStream(data);
                    using var br = new BinaryReader(stream);
                    using var bw = new BinaryWriter(new FileStream(filename, FileMode.Create));
                    byte[] buffer = new byte[2048];
                    int length = 0;
                    while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, length);
                    }
                }
                catch (Exception exc)
                {
                    var msg = exc.Message;
                }
                finally
                {
                    tcs.SetResult(true);
                }
            }).ConfigureAwait(false);

            return tcs.Task;
        }
    }
}
