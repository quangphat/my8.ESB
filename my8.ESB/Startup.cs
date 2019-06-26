
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
        //public IServiceProvider ConfigureServices(IServiceCollection services)
        //{
        //    services.AddMvc();

        //    //By connecting here we are making sure that our service
        //    //cannot start until redis is ready. This might slow down startup,
        //    //but given that there is a delay on resolving the ip address
        //    //and then creating the connection it seems reasonable to move
        //    //that cost to startup instead of having the first request pay the
        //    //penalty.
        //    //services.AddSingleton(sp =>
        //    //{
        //    //    var configuration = new ConfigurationOptions {ResolveDns = true};
        //    //    configuration.EndPoints.Add(Configuration["RedisHost"]);
        //    //    return ConnectionMultiplexer.Connect(configuration);
        //    //});

        //    //services.AddTransient<IIdentityRepository, IdentityRepository>();
        //    var builder = new ContainerBuilder();

        //    // register a specific consumer
        //    builder.RegisterType<ApplicantAppliedEventConsumer>();

        //    builder.Register(context =>
        //    {
        //        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        //        {
        //            var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
        //            {
        //                h.Username("quangphat");
        //                h.Password("number8");
        //            });
        //            //cfg.Durable = false;
        //            //cfg.AutoDelete = true;
        //            // https://stackoverflow.com/questions/39573721/disable-round-robin-pattern-and-use-fanout-on-masstransit
        //            cfg.ReceiveEndpoint(host, "test_queue", e =>
        //            {
        //                //e.LoadFrom(context);
        //                e.Consumer<ApplicantAppliedEventConsumer>();
        //            });
        //        });

        //        return busControl;
        //    })
        //        .SingleInstance()
        //        .As<IBusControl>()
        //        .As<IBus>();

        //    builder.Populate(services);
        //    ApplicationContainer = builder.Build();
        //    return new AutofacServiceProvider(ApplicationContainer);
        //}
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            services.Configure<BusConfig>(Configuration.GetSection("Bus"));

            services.AddScoped<CommentHandler>();
            services.AddScoped<JobHandler>();
            services.AddSingleton<IBusControl>(u =>
            {
                var config = u.GetService<IOptions<BusConfig>>().Value;
                return Bus.Factory.CreateUsingRabbitMq(
                            config,
                            (queue, handler) => handlerConfig(queue, u, handler));
            });
            services.AddMassTransit(opt =>
            {
                opt.AddConsumer<CommentHandler>();
                opt.AddConsumer<JobHandler>();
            });

            services.AddSingleton<IHostedService, BusService>();
        }

        private void handlerConfig(string queue, IServiceProvider provider, MassTransit.RabbitMqTransport.IRabbitMqReceiveEndpointConfigurator handler)
        {
            switch (queue)
            {
                case "comment_queue":
                    handler.Consumer<CommentHandler>(provider);
                    //EndpointConvention.Map<DoSomething>(handler.InputAddress);
                    break;
                case "job_queue":
                    handler.Consumer<JobHandler>(provider);
                    //EndpointConvention.Map<DoSomething>(handler.InputAddress);
                    break;
                default:
                    //handler.Consumer<CommentHandler>(provider);
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
            //app.UseMvc();
            //var bus = ApplicationContainer.Resolve<IBusControl>();
            //var busHandle = TaskUtil.Await(() => bus.StartAsync());
            //lifetime.ApplicationStopping.Register(() => busHandle.Stop());
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
