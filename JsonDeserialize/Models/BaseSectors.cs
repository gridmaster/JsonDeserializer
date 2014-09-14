using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public class BaseSectors
    {
        [JsonProperty(PropertyName = "sector")]
        public List<Sectors> Sectors { get; set; }
    }

}
