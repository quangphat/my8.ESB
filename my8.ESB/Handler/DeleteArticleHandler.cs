using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Handler
{
    public class DeleteArticleHandler : IConsumer<DoSomething>
    {

        public DeleteArticleHandler()
        {
        }
        public async Task Consume(ConsumeContext<DoSomething> message)
        {
            Console.WriteLine("success");
            return;
        }
    }
    public class ArticleDeleteMsg
    {
        public ArticleDeleteMsg() { }

        public long Id { get; set; }
        public long BlogId { get; set; }
        public long StoreId { get; set; }
    }
}
