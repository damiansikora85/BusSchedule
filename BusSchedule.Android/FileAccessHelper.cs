using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BusSchedule.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(BusSchedule.Droid.FileAccessHelper))]
namespace BusSchedule.Droid
{
    public class FileAccessHelper : IFileAccess
	{
		public string GetLocalFilePath(string filename)
		{
			string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			string dbPath = Path.Combine(path, filename);

			CopyDatabaseIfNotExists(dbPath, filename);

			return dbPath;
		}

		private void CopyDatabaseIfNotExists(string dbPath, string filename)
		{
			if (!File.Exists(dbPath))
			{
				using (var br = new BinaryReader(Android.App.Application.Context.Assets.Open(filename)))
				{
					using (var bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
					{
						byte[] buffer = new byte[2048];
						int length = 0;
						while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
						{
							bw.Write(buffer, 0, length);
						}
					}
				}
			}
		}
	}
}