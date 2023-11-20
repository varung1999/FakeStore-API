using Newtonsoft.Json;

namespace FakeStoreTwo.Models
{
    public class RatingModel
    {
        [JsonProperty("rate")]
        public float Rate { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }  
    }
}