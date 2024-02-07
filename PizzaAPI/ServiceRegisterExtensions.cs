using Core.Data.Repositories;
using Data.Db.Network;
using Data.Db.Repositories.Interfaces;

namespace PizzaAPI
{
    public static class ServiceRegisterExtensions
    {
        public static void AddDevServices(this IServiceCollection services)
        {
            // Configuration
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            services.Configure<DbSettings>(config.GetSection("DbSettings"));
            services.AddOptions();

            // MediatR
            services.AddMediatRToAssemblies();

            // Data access services
            services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
            services.AddSingleton<IToppingRepo, ToppingRepo>();
            services.AddSingleton<IPizzaRepo, PizzaRepo>();
            services.AddSingleton<IOrderRepo, OrderRepo>();
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
    }
}
