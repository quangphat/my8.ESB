using MassTransit;
using my8.ESB.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Handler
{
    public class CommentHandler : IConsumer<CommentMsg>
    {
        public async Task Consume(ConsumeContext<CommentMsg> context)
        {
            string value = context.Message.Comment;
        }
    }
}
