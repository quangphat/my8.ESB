
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using my8.ESB.Handler;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace my8.ESB
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<BusConfig>(Configuration.GetSection("Bus"));
            services.AddScoped<ApplicantAppliedEventConsumer>();
            services.AddSingleton<IBusControl>(u =>
            {
                var config = u.GetService<IOptions<BusConfig>>().Value;
                return Bus.Factory.CreateUsingRabbitMq(
                            config,
                            (queue, handler) => handlerConfig(queue, u, handler));
            });
            services.AddMassTransit(opt =>
            {
                opt.AddConsumer<ApplicantAppliedEventConsumer>();
            });
            
            services.AddSingleton<IHostedService, BusService>();
        }

        private void handlerConfig(string queue, IServiceProvider provider, MassTransit.RabbitMqTransport.IRabbitMqReceiveEndpointConfigurator handler)
        {
            switch (queue)
            {
                case "test_queue":
                    handler.Consumer<ApplicantAppliedEventConsumer>(provider);
                    //EndpointConvention.Map<DoSomething>(handler.InputAddress);
                    break;
                default:
                    handler.Consumer<ApplicantAppliedEventConsumer>(provider);
                    break;
            }
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }

    }
    public class BusService : IHostedService
    {
        private readonly IBusControl _busControl;

        public BusService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _busControl.StopAsync(cancellationToken);
        }
    }
    public class YourMessage
    {
        public string Text { get; set; }
    }
}
