using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public class Ticker : BaseTicker
    {
        [JsonProperty(PropertyName = "Symbol")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "Name")]
	    public string Name { get; set; }

        [JsonProperty(PropertyName = "ExchangeName")]
	    public string ExchangeName { get; set; }

        [JsonProperty(PropertyName = "Industry")]
	    public string Industry { get; set; }

        [JsonProperty(PropertyName = "Sector")]
        public string Sector { get; set; }
    }
}
