using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using my8.ESB.Models;
using my8.ESB.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Infrastructures
{
    public class MapConfigs
    {
        public static void Config(IServiceCollection services)
        {
            Mapper.Initialize(mapper =>
            {
                ConfigMapper(mapper);
            });
        }
        public static void ConfigMapper(IMapperConfigurationExpression mapper)
        {
            mapper.AllowNullCollections = true;
            mapper.CreateMap<ObjectId, string>().ConvertUsing(a => a.ToString());
            mapper.CreateMap<string, ObjectId>().ConvertUsing(a => ObjectId.Parse(a));
            mapper.CreateMap<SearchModelMsg, SearchModel>()
               .ForMember(a => a.Id, b => b.MapFrom(c => c.Id))
               .ForMember(a => a.ObjectType, b => b.MapFrom(c => c.ObjectType));
            
        }
    }
}
