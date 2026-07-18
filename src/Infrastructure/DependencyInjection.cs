using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Infrastructure;
using Application.Common.Interfaces.ReadRepositories;
using Domain.Repositories;
using Infrastructure.Authentication;
using Infrastructure.BackgroundJobs;
using Infrastructure.Events;
using Infrastructure.ExternalServices.Payment;
using Infrastructure.Identity;
using Infrastructure.Outbox;
using Infrastructure.Persistence.ReadContext;
using Infrastructure.Persistence.ReadContext.Repositories;
using Infrastructure.Persistence.Seeding;
using Infrastructure.Persistence.WriteContext.Context;
using Infrastructure.Persistence.WriteContext.Interceptors;
using Infrastructure.Persistence.WriteContext.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Identity & Authentication & Authorization
        services.AddIdentity()
            .AddJwtAuthentication(configuration)
            .AddAuthorization();
        
        // Add DbContext, and Repositories & Unit of Work
        services.AddDbContext(configuration)
            .AddRepositories(configuration);

        services.AddExternalServices(configuration);

        // Add Background Jobs
        services.AddBackgroundJobs(configuration);

        services.AddOtherServices(configuration);

        return services;
    }

    private static IServiceCollection AddOtherServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CurrentUserService>();
        services.AddScoped<IUserContext, CurrentUserService>();
        services.AddSingleton<OutboxChannel>();
        
        services.AddScoped<DomainEventDispatcher>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICourseReadRepository, CourseReadRepository>();
        services.AddScoped<IUserEnrollmentReadRepository, UserEnrollmentReadRepository>();
        services.AddScoped<IPaymentReadRepository, PaymentReadRepository>();

        return services;
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Wtite Db
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DomainEventToOutboxInterceptor>();

        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("WriteDb"),
                b =>
                {
                    b.MigrationsAssembly(typeof(WriteDbContext).Assembly.FullName);
                    b.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                });

            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            
            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
                serviceProvider.GetRequiredService<DomainEventToOutboxInterceptor>());
        });

        services.AddScoped<DatabaseSeeder>();

        // Read Db
        services.AddScoped<MongoDbContext>();
        MongoDbContext.Configure();

        return services;
    }

    private static IServiceCollection AddIdentity(
        this IServiceCollection services)
    {
        services.AddIdentity<Participant, IdentityRole<Guid>>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<WriteDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        var jwtSettings = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;

        services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(jwtSettings.TokenLifespanHours));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero, // Remove default 5 min clock skew

                NameClaimType = JwtRegisteredClaimNames.Sub, // Important for UserIdentifier
            };
        });

        services.AddScoped<JwtTokenGenerator>();

        return services;
    }

    private static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStripe(configuration);

        return services;
    }

    private static IServiceCollection AddStripe(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
        
        var stripeSettings = configuration.GetSection("Stripe").Get<StripeOptions>();
        Stripe.StripeConfiguration.ApiKey = stripeSettings!.SecretKey;

        services.AddScoped<IPaymentService, StripePaymentService>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<RefreshTokenCleanupJob>();
        services.Configure<RefreshTokenCleanupOptions>(configuration.GetSection(RefreshTokenCleanupOptions.SectionName));

        services.AddHostedService<OutboxBackgroundService>();
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));

        return services;
    }
}
