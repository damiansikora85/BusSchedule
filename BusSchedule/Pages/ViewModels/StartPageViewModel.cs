using BusSchedule.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BusSchedule.Pages.ViewModels
{
    class StartPageViewModel
    {
        internal void CheckIfNeedUpdateData()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(LoadResourceText)).Assembly;
            var stream = assembly.GetManifestResourceStream("BusSchedule.data.json");
            string jsonData = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                jsonData = reader.ReadToEnd();
            }
            var data = JsonConvert.DeserializeObject<ScheduleData>(jsonData);
            var version = data.Version;
        }
    }
}
