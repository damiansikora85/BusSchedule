using Newtonsoft.Json;
using System;

namespace BusSchedule.Core.Model
{
    public class ElectronicCardData
    {
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("issuer_id")]
        public string IssuerId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("valid_to")]
        public DateTime ValidTo { get; set; }
        [JsonProperty("discount_valid_to")]
        public DateTime DiscountValidTo { get; set; }
    }
}
