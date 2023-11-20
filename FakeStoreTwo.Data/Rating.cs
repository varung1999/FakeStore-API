using MongoDB.Bson.Serialization.Attributes;

namespace FakeStoreTwo.Data
{
    public class Rating
    {
        [BsonElement("rate")]
        public float Rate { get; set; }

        [BsonElement("count")]
        public int Count { get; set; }
    }
}