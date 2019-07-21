using Microsoft.Extensions.Options;
using MongoDB.Driver;
using my8.ESB.Infrastructures;
using my8.ESB.IRepository;
using my8.ESB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Repository
{
    public class RecommendedTagRepository : MongoRepositoryBase<RecommendedTag>, IRecommendedTagRepository
    {
        IMongoCollection<RecommendedTag> collection;
        FilterDefinition<RecommendedTag> filter = FilterDefinition<RecommendedTag>.Empty;
        public RecommendedTagRepository(IOptions<MongoConnection> setting) : base(setting)
        {
            collection = _db.GetCollection<RecommendedTag>("RecommendedTag");
        }

        public async Task<bool> UpdateCountUsed(string tagId, int value)
        {
            var filter = Builders<RecommendedTag>.Filter.Eq(p => p.Id, tagId);
            var update = Builders<RecommendedTag>.Update
                            .Inc(s => s.CountUsed, value);
            await collection.UpdateOneAsync(filter, update);
            return true;
        }
        public async Task<bool> UpdateCountUsed(string[] tags,int value)
        {
            filter = Builders<RecommendedTag>.Filter.In(p => p.Tag, tags);
            var update = Builders<RecommendedTag>.Update
                            .Inc(s => s.CountUsed,value );
            await collection.UpdateManyAsync(filter, update);
            return true;
        }
    }
}
