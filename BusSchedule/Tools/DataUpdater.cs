using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.Pages.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BusSchedule.Tools
{
    public class DataUpdater
    {
        public static void SetupData(IDataProvider dataProvider)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(LoadResourceText)).Assembly;
            var stream = assembly.GetManifestResourceStream("BusSchedule.data.json");
            string jsonData = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                jsonData = reader.ReadToEnd();
            }

            var schedule = JsonConvert.DeserializeObject<ScheduleData>(jsonData);
            dataProvider.UpdateAsync(schedule);
        }
    }
}
