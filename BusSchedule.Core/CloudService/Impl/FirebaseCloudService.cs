using BusSchedule.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                var jsonObject = JObject.Parse(result);
                return jsonObject["filename"] != null ? jsonObject["filename"].ToString() : string.Empty;
            }
            return "";
        }

        public async Task<List<News>> GetNews()
        {
            var response = await _client.GetAsync("https://us-central1-busschedule-4d81f.cloudfunctions.net/getNews");
            if(response.IsSuccessStatusCode)
            {
                var newsJson = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<List<News>>(newsJson);
                news.Reverse();
                return news;
            }
            return Enumerable.Empty<News>().ToList();
        }
    }
}
