using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public class BaseIndustries
    {
        [JsonProperty(PropertyName = "industry")]
        public List<Industry> Industries { get; set; }
    }
}
