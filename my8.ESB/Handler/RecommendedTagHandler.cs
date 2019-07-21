using MassTransit;
using my8.ESB.IRepository;
using my8.ShareObject.ESB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Handler
{
    public class RecommendedTagHandler : IConsumer<RecommendedTagMsg>
    {
        protected readonly IRecommendedTagRepository _rpRecommendedTag;
        public RecommendedTagHandler(IRecommendedTagRepository recommendedTagRepository)
        {
            this._rpRecommendedTag = recommendedTagRepository;
        }
        public async Task Consume(ConsumeContext<RecommendedTagMsg> context)
        {
            if (context.Message.Tags == null)
                return;
            await HandleUpdateTagUsedCount(context.Message.Tags);
        }
        private async Task HandleUpdateTagUsedCount(List<string> tags)
        {
            await _rpRecommendedTag.UpdateCountUsed(tags.ToArray(), 1);
        }
    }
}
