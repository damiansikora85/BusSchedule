﻿using BusSchedule.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BusSchedule.Droid.FileAccessHelper))]
namespace BusSchedule.Droid
{
    public class FileAccessHelper : IFileAccess
	{
		public string GetLocalFilePath(string filename)
		{
			string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			string filenamePath = Path.Combine(path, filename);
			return filenamePath;
		}

        public bool CheckLocalFileExist(string filename)
        {
            var localFilename = GetLocalFilePath(filename);
            return File.Exists(localFilename);
        }

        public async Task<string> ReadAssetFile(string filename)
        {
            using StreamReader sr = new StreamReader(Android.App.Application.Context.Assets.Open(filename));
            return await sr.ReadToEndAsync();
        }

        public Task<bool> CopyFromAssetsToLocal(string destFilename, string assetFilename)
		{
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task.Run(() =>
            {
                try
                {
                    if (File.Exists(destFilename))
                    {
                        File.Delete(destFilename);
                    }
                    using var br = new BinaryReader(Android.App.Application.Context.Assets.Open(assetFilename));
                    using var bw = new BinaryWriter(new FileStream(destFilename, FileMode.Create));
                    byte[] buffer = new byte[2048];
                    int length = 0;
                    while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, length);
                    }
                }
                catch(Exception exc)
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