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
    public class SearchModelRepository : MongoRepositoryBase<SearchModel>, ISearchModelRepository
    {
        IMongoCollection<SearchModel> collection;
        FilterDefinition<SearchModel> filter = FilterDefinition<SearchModel>.Empty;
        public SearchModelRepository(IOptions<MongoConnection> setting) : base(setting)
        {
            collection = _db.GetCollection<SearchModel>("SearchModel");
        }

        public async Task<string> Create(SearchModel model)
        {
            await collection.InsertOneAsync(model);
            return model.Id;
        }
    }
}
