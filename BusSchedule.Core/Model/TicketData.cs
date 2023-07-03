using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class TicketData
    {
        [JsonProperty("issuer_name")]
        public string IssuerName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price")]
        public float Price { get; set; }
        [JsonProperty("valid_since")]
        public DateTime ValidSince { get; set; }
        [JsonProperty("valid_for")]
        public DateTime ValidFor { get; set; }
        [JsonProperty("place")]
        public string Place { get; set; }
        [JsonProperty("zone")]
        public string Zone { get; set; }
        [JsonProperty("transaction_time")]
        public DateTime TransactionTime { get; set; }
        [JsonProperty("issuer_id")]
        public int IssuerId { get; set; }
        public string Valid => $"{ValidSince:dd.MM.yyyy} - {ValidFor:dd.MM.yyyy}";
    }
}
