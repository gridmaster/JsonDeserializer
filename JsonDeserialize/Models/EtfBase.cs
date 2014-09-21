using System;
using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public abstract class EtfBase : BaseSymbol
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        // ETF NAME
        [JsonProperty(PropertyName = "EtfName")]
        public string EtfName { get; set; }

        //TICKER
        [JsonProperty(PropertyName = "Ticker")]
        public string Ticker { get; set; }

        //CATEGORY
        [JsonProperty(PropertyName = "Category")]
        public string Category { get; set; }

        //FUND FAMILY
        [JsonProperty(PropertyName = "FundFamily")]
        public string FundFamily { get; set; }
    }
}
