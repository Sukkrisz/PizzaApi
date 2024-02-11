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

            // Api related injections
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger
            builder.Services.AddSwaggerGen(c => c.CustomSchemaIds(t => t.FullName.Replace("+", ".")));

            // MediatR
            builder.Services.AddMediatRToAssemblies();

            // App related services
            builder.Services.AddDevServices(config);

            // In memory cache
            builder.Services.AddOutputCache(options =>
            {
                // Set the cache expiry to 1 minutes
                options.AddBasePolicy(x => x.Expire(TimeSpan.FromMinutes(1)));

                // Only those endpoints will be cached, where it's explicitly set
                options.AddBasePolicy(x => x.NoCache());
            });


            // Redis cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("Redis");
                options.InstanceName = "PizzaApi_";
            });

            var app = builder.Build();

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

            app.Run();
        }
    }
}
