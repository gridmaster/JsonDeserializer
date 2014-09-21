using System.Runtime.Serialization;

namespace JsonDeserialize.Models
{
    public class BaseSymbol
    {
        protected string dateForSerialization;

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            this.dateForSerialization = "1900-01-01";
        }
    }
}
