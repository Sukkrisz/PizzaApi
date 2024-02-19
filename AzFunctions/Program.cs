using Core.Data.Repositories;
using Data.Db.DbAccess;
using Data.Db.Repositories.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main()
    {
        var configBuilder = new ConfigurationBuilder();

#if DEBUG
        // In debug we read the settings from this file, in azure, this file is not present.
        configBuilder.AddJsonFile("local.settings.json");
#endif

        configBuilder.AddEnvironmentVariables();
        IConfigurationRoot config = configBuilder.Build();

        IHostBuilder hostbuilder = new HostBuilder();

        hostbuilder.ConfigureFunctionsWorkerDefaults();

        hostbuilder.ConfigureServices(services =>
        {
            services.Configure<ConnectionStringSettings>(config.GetSection("Db"));
            services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
            services.AddSingleton<IOrderRepo, OrderRepo>();
            services.AddSingleton<IPizzaRepo, PizzaRepo>();
            services.AddOptions();
        });

        var host = hostbuilder.Build();

        host.Run();
    }
}
