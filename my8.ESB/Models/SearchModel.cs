using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace my8.ESB.Models
{
    public class SearchModel
    {
        public SearchModel() { }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FeedId { get; set; }
        public int ObjectType { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; }
        public string ProjectId { get; set; }
    }
}
