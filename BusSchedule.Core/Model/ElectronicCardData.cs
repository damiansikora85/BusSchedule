using Newtonsoft.Json;
using System;

namespace BusSchedule.Core.Model
{
    public class ElectronicCardData
    {
        private string _number;

        [JsonProperty("number")]
        public string Number
        {
            get => _number;
            set => _number = value.PadLeft(10, '0');
        }
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
