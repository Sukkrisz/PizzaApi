using Infrastructure.Settings;
using Infrastructure.Blob;
using Azure.Messaging.ServiceBus;
using Infrastructure.ServiceBus;
using Database.DbAccess;
using Database.Repositories;
using Database.Repositories.Interfaces;
using Infrastructure.Blob.Interfaces;

namespace PizzaAPI
{
    public static class ServiceRegisterExtensions
    {
        public static void AddDevServices(this IServiceCollection services, IConfigurationRoot config)
        {
            services.Configure<ConnectionStringSettings>(config.GetSection("ConnectionStrings"));
            services.Configure<AzServiceBusSettings>(config.GetSection("AzServiceBus"));
            services.AddOptions();

            // Data access services
            services.AddTransient<ISqlDataAccess, SqlDataAccess>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IToppingRepo, ToppingRepo>();
            services.AddTransient<IPizzaRepo, PizzaRepo>();
            services.AddTransient<IOrderRepo, OrderRepo>();
            services.AddTransient<IBusMessagePublisher, BusMessagePublisher>();
        }

        public static IServiceCollection AddMediatRToAssemblies(this IServiceCollection services)
        {
            var projectsToAddTo = new string[] { "Data", "PizzaAPI" };
            var assembliesToAddTo = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                                                                                    !a.IsDynamic &&
                                                                                    projectsToAddTo.Any(p => a.FullName.StartsWith(p)));

            foreach (var assembly in assembliesToAddTo)
            {
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            }

            return services;
        }

        public static void AddServiceBus(this IServiceCollection services, IConfigurationRoot config)
        {
            var connString = config.GetConnectionString("AZServiceBus");
            var options = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpTcp
            };

            services.AddSingleton(x => new ServiceBusClient(connString, options));
        }
    }
}
