using Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.Handler
{
    public class ApplicantAppliedEventConsumer : IConsumer<ApplicantAppliedEvent>
    {
        //private readonly IIdentityRepository _identityRepository;

        public ApplicantAppliedEventConsumer()
        {

        }

        public async Task Consume(ConsumeContext<ApplicantAppliedEvent> context)
        {
            int x = 10;
            // increment the user's application count in the cache
            //await _identityRepository.UpdateUserApplicationCountAsync(context.Message.ApplicantId.ToString());
        }
    }
}
