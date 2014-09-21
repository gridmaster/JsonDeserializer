using Newtonsoft.Json;

namespace JsonDeserialize.Models
{
    public abstract class BaseTicker : BaseSymbol
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

    }
}
