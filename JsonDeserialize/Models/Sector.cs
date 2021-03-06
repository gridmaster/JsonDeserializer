﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public class Sector : BaseTicker
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "industry")]
        public List<Industry> Industries { get; set; }
    }
}
