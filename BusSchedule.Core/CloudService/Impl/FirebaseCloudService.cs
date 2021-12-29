﻿using BusSchedule.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.CloudService.Impl
{
    public class FirebaseCloudService : ICloudService
    {
        private HttpClient _client;

        public FirebaseCloudService()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetLatestScheduleFilename()
        {
            var response = await _client.GetAsync("https://us-central1-busschedule-4d81f.cloudfunctions.net/latestSchedule");
            var result = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }

        public async Task<string> TestGet()
        {
            var response = await _client.GetAsync("https://us-central1-busschedule-4d81f.cloudfunctions.net/helloWorld");
            var result = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }

        public async Task<List<News>> GetNews()
        {
            var response = await _client.GetAsync("https://us-central1-busschedule-4d81f.cloudfunctions.net/getNews");
            if(response.IsSuccessStatusCode)
            {
                var newsJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<News>>(newsJson);
            }
            return Enumerable.Empty<News>().ToList();
        }
    }
}