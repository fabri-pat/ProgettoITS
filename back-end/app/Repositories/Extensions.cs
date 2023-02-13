using app.Helpers;
using MongoDB.Driver;

namespace app.Repositories
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            services.AddScoped(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var connectionString = configuration!.GetSection("MongoSettings:ConnectionString").Value;
                var databaseName = configuration.GetSection("MongoSettings:DatabaseName").Value;
                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                var mongoClient = new MongoClient(settings);

                return mongoClient.GetDatabase(databaseName);
            });

            return services;
        }
    }
}