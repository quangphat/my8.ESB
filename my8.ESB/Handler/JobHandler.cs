using MassTransit;
using my8.ShareObject.ESB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Handler
{
    public class JobHandler : IConsumer<JobMsg>
    {
        public async Task Consume(ConsumeContext<JobMsg> context)
        {
            string title = context.Message.Title;
        }
        
    }
}
