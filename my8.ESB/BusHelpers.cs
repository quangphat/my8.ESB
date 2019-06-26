using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB
{
    public static class BusHelpers
    {
        public static IBusControl CreateUsingRabbitMq(
     this IBusFactorySelector factory,
     BusConfig config,
     Action<string, IRabbitMqReceiveEndpointConfigurator> handler = null)
        {
            return BusFactoryConfiguratorExtensions.CreateUsingRabbitMq(Bus.Factory, (Action<IRabbitMqBusFactoryConfigurator>)(sbc =>
            {
                IRabbitMqHost irabbitMqHost = RabbitMqHostConfigurationExtensions.Host(sbc, new Uri(config.Host), (Action<IRabbitMqHostConfigurator>)(h =>
                {
                    h.Username(config.User);
                    h.Password(config.Pass);
                }));
                if (handler != null && config.Handlers != null && ((IEnumerable<BusConfigHandler>)config.Handlers).Any<BusConfigHandler>())
                {
                    foreach (BusConfigHandler handler1 in config.Handlers)
                    {
                        BusConfigHandler handlerConfig = handler1;
                        sbc.ReceiveEndpoint(irabbitMqHost, handlerConfig.Queue, (Action<IRabbitMqReceiveEndpointConfigurator>)(ep =>
                        {
                            ((IQueueEndpointConfigurator)ep).PrefetchCount =(ushort)handlerConfig.Concurency;
                            handler(handlerConfig.Queue, ep);
                        }));
                    }
                }
                //ExtensionsLoggingConfiguratorExtensions.UseExtensionsLogging((IBusFactoryConfigurator)sbc, loggerFactory);
                //MessageSchedulerExtensions.UseMessageScheduler((IPipeConfigurator<ConsumeContext>)sbc, new Uri(config.Host + "/Bus_JobQuartz"));
            }));
        }
    }
}
