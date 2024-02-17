using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

namespace PizzaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Read the config file
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            AddServices(builder.Services, config);

            builder.Logging.AddConsole();

            var app = builder.Build();

            UseServices(app);

            app.Run();
        }

        private static void AddServices(IServiceCollection services, IConfigurationRoot config)
        {
            // Api related injections
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Swagger
            services.AddSwaggerGen(c => c.CustomSchemaIds(t => t.FullName.Replace("+", ".")));

            // MediatR
            services.AddMediatRToAssemblies();

            // Blolb storage
            services.AddSingleton(x => new BlobServiceClient(config.GetConnectionString("Blob")));

            // Azure service bus
            services.AddServiceBus(config);

            // App related services
            services.AddDevServices(config);

            // In memory cache
            services.AddOutputCache(options =>
            {
                // Set the cache expiry to 1 minutes
                options.AddBasePolicy(x => x.Expire(TimeSpan.FromMinutes(1)));

                // Only those endpoints will be cached, where it's explicitly set
                options.AddBasePolicy(x => x.NoCache());
            });


            // Redis cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("Redis");
                options.InstanceName = "PizzaApi_";
            });
        }

        private static void UseServices(WebApplication app)
        {
            app.UseOutputCache();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
