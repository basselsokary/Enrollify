// using Infrastructure.Persistence.WriteContext.Interceptors;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;

// namespace Infrastructure.Persistence.WriteContext.Context;

// internal class WriteDbContextFactory(
//     IConfiguration configuration,
//     IServiceProvider serviceProvider) : IDesignTimeDbContextFactory<WriteDbContext>
// {
//     public WriteDbContext CreateDbContext(string[] args)
//     {
//         var optionsBuilder = new DbContextOptionsBuilder<WriteDbContext>();
//             ConfigureDbContext(optionsBuilder, configuration);
//             optionsBuilder.AddInterceptors(
//                 serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
//                 serviceProvider.GetRequiredService<DomainEventDispatcherInterceptor>()
//             );

//         return new WriteDbContext(optionsBuilder.Options);
//     }

//     private static void ConfigureDbContext(
//         DbContextOptionsBuilder options,
//         IConfiguration configuration)
//     {
//         options.UseNpgsql(
//             configuration.GetConnectionString("WriteDb"),
//             b =>
//             {
//                 b.MigrationsAssembly(typeof(WriteDbContext).Assembly.FullName);
//                 b.EnableRetryOnFailure(
//                     maxRetryCount: 3,
//                     maxRetryDelay: TimeSpan.FromSeconds(5),
//                     errorCodesToAdd: null);
//             });

//         options.EnableSensitiveDataLogging(false);
//         options.EnableDetailedErrors(false);
//     }
// }