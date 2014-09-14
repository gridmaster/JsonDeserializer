using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public class Company
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "symbol")]
        public string Symbol { get; set; }
    }
}
