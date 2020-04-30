using MassTransit;
using my8.ESB.IRepository;
using my8.ESB.Models;
using my8.ESB.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Handler
{
    public class SearchModelHandler : IConsumer<SearchModelMsg>
    {
        ISearchModelRepository _rpSearchModel;
        public SearchModelHandler(ISearchModelRepository searchModelRepository)
        {
            this._rpSearchModel = searchModelRepository;
        }
        public async Task Consume(ConsumeContext<SearchModelMsg> context)
        {
            var msg = context.Message;
            if (msg == null)
                return;
            await CreateSearch(msg);
        }
        public async Task CreateSearch(SearchModelMsg msg)
        {
            var model = AutoMapper.Mapper.Map<SearchModel>(msg);
            await _rpSearchModel.Create(model);
        }
    }
}
