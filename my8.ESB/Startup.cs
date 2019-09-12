
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using my8.ESB.Handler;
using my8.ESB.Infrastructures;
using my8.ESB.IRepository;
using my8.ESB.Models;
using my8.ESB.Repository;
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
            services.Configure<MongoConnection>(Configuration.GetSection("MongoConnection"));
            services.Configure<BusConfig>(Configuration.GetSection("Bus"));
            MapConfigs.Config(services);
            services.AddSingleton<IRecommendedTagRepository, RecommendedTagRepository>();
            services.AddSingleton<ISearchModelRepository, SearchModelRepository>();
            services.AddScoped<CommentHandler>();
            services.AddScoped<RecommendedTagHandler>();
            services.AddScoped<SearchModelHandler>();
            services.AddSingleton(u =>
            {
                var config = u.GetService<IOptions<BusConfig>>().Value;
                return Bus.Factory.CreateUsingRabbitMq(
                            config,
                            (queue, handler) => handlerConfig(queue, u, handler));
            });
            services.AddMassTransit(opt =>
            {
                opt.AddConsumer<CommentHandler>();

            });

            services.AddSingleton<IHostedService, BusService>();
        }

        private void handlerConfig(string queue, IServiceProvider provider, MassTransit.RabbitMqTransport.IRabbitMqReceiveEndpointConfigurator handler)
        {
            switch (queue)
            {
                case "comment_queue":
                    handler.Consumer<CommentHandler>(provider);
                    break;
                case "recommendedtag_queue":
                    handler.Consumer<RecommendedTagHandler>(provider);
                    break;
                case "search_model":
                    handler.Consumer<SearchModelHandler>(provider);
                    break;
                default:
                    break;
            }
        }
        public IContainer ApplicationContainer { get; private set; }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
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
}
