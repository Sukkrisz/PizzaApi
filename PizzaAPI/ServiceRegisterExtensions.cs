using Core.Data.Repositories;
using Infrastructure.Settings;
using Data.Db.Repositories.Interfaces;
using Infrastructure.Blob;

namespace PizzaAPI
{
    public static class ServiceRegisterExtensions
    {
        public static void AddDevServices(this IServiceCollection services, IConfigurationRoot config)
        {
            services.Configure<ConnectionStringSettings>(config.GetSection("ConnectionStrings"));
            services.AddOptions();

            // Data access services
            services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
            services.AddSingleton<IFileService, FileService>();
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
