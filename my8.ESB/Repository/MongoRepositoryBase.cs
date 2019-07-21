using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using my8.ESB.Infrastructures;
using my8.ESB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Repository
{
    public abstract class MongoRepositoryBase<TEntity> where TEntity : class
    {
        protected MongoClient _client;
        protected IMongoDatabase _db { get; set; }
        MongoConnection mongoConnection;
        internal readonly IBsonSerializerRegistry _serializerRegistry;
        internal readonly IBsonSerializer<TEntity> _documentSerializer;
        public MongoRepositoryBase(IOptions<MongoConnection> setting)
        {
            mongoConnection = setting.Value;
            Connect(mongoConnection);
            _serializerRegistry = BsonSerializer.SerializerRegistry;
            _documentSerializer = _serializerRegistry.GetSerializer<TEntity>();
        }
        private void Connect(MongoConnection mongoConnection)
        {
            _client = new MongoClient($"mongodb://{mongoConnection.UserName}:{mongoConnection.Password}@{mongoConnection.ServerURl}");
            if (_db == null)
                _db = _client.GetDatabase(mongoConnection.Database);
        }
        protected async Task<MGPagination<TEntity>> GetPaginationAsync(IMongoCollection<TEntity> collection,
            int page, int pageSize,
            FilterDefinition<TEntity> filter = null,
            SortDefinition<TEntity> sort = null,
            ProjectionDefinition<TEntity> projection = null)
        {
            var pipeline = new List<BsonDocument>();
            if (filter != null)
                pipeline.Add(new BsonDocument { { "$match", filter.Render(_documentSerializer, _serializerRegistry) } });
            if (sort != null)
                pipeline.Add(new BsonDocument { { "$sort", sort.Render(_documentSerializer, _serializerRegistry) } });
            if (projection != null)
            {
                //var projectionBuilder = Builders<TEntity>.Projection.Expression(projection);

                //pipeline.Add(new BsonDocument { { "$project", projection.Render(_documentSerializer, _serializerRegistry).Document } });
                pipeline.Add(new BsonDocument { { "$project", projection.Render(_documentSerializer, _serializerRegistry) } });
            }

            pipeline.Add(new BsonDocument
            {
                {
                    "$group", new BsonDocument
                    {
                        { "_id", 0 },
                        { "total", new BsonDocument { { "$sum", 1 } } },
                        { "datas", new BsonDocument { { "$push", "$$ROOT" } } }
                    }
                }
            });

            pipeline.Add(new BsonDocument
            {
                {
                    "$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "total", 1 },
                        { "datas", new BsonDocument { { "$slice", new BsonArray(new object[] { "$datas", (page-1) * pageSize, pageSize }) } } }
                    }
                }
            });

            var aggregate = collection.Aggregate<BsonDocument>(pipeline);

            var data = await aggregate.FirstOrDefaultAsync();

            var result = data == null
                ? new MGPagination<TEntity>() { datas = new TEntity[0], total = 0 }
                : BsonSerializer.Deserialize<MGPagination<TEntity>>(data);

            return result;
        }

    }
    [BsonIgnoreExtraElements]
    public class MGPagination<TEntity>
    {
        public int total { get; set; }
        public IEnumerable<TEntity> datas { get; set; }
    }
}
