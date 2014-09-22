using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public class Industry : BaseTicker
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "company")]
        public List<Company> Companies { get; set; }
    }
}
