using Infrastructure.Persistence.ReadContext;
using Infrastructure.Persistence.Seeding;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class StaticDataServiceProviderExtensions
{
    public static async Task SeedAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(cancellationToken);
        
        var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        await MongoDbInitializer.InitializeAsync(mongoDbContext);
    }
}
