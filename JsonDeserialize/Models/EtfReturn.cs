using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public class EtfReturn : EtfBase
    {
        public EtfReturn()
        {
        }

        //INTRADAY RETURN	
        [JsonProperty(PropertyName = "IntradayReturn")]
        public string IntradayReturn { get; set; }

        //3-MO RETURN	
        [JsonProperty(PropertyName = "ThreeMoReturn")]
        public string ThreeMoReturn { get; set; }

        //YTD RETURN	
        [JsonProperty(PropertyName = "YTDReturn")]
        public string YTDReturn { get; set; }

        //1-YR RETURN	
        [JsonProperty(PropertyName = "OneYrReturn")]
        public string OneYrReturn { get; set; }

        //3-YR RETURN	
        [JsonProperty(PropertyName = "ThreeYrReturn")]
        public string ThreeYrReturn { get; set; }

        //5-YR RETURN
        [JsonProperty(PropertyName = "FiveYrReturn")]
        public string FiveYrReturn { get; set; }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            base.dateForSerialization = this.Date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
