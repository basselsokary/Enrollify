using Domain.Common.Constants;
using Infrastructure.Identity;
using Infrastructure.Outbox;
using Infrastructure.Persistence.WriteContext.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Seeding;

/// <summary>
/// Orchestrates all <see cref="IDataSeeder"/> implementations in the correct order.
/// Registered as a scoped service and invoked once during application startup.
/// </summary>
internal sealed class DatabaseSeeder(
    WriteDbContext context,
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<Participant> userManager,
    OutboxChannel outboxChannel,
    IConfiguration configuration)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.MigrateAsync(cancellationToken);

        if (!roleManager.Roles.Any())
        {
            await EnsureRolesAsync(roleManager);
            await SeedInitialUserAsync(userManager, configuration);
        }

        await CourseSeeder.SeedAsync(context, outboxChannel, cancellationToken);
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var existingRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
        
        foreach (string roleName in UserRoles.AllRoles)
        {
            bool exists = existingRoles.Contains(roleName);
            if (!exists)
            {
                IdentityResult createRoleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                if (!createRoleResult.Succeeded)
                {
                    string errors = string.Join(", ", createRoleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create role '{roleName}': {errors}");
                }
            }
        }
    }

    private static async Task SeedInitialUserAsync(
        UserManager<Participant> userManager,
        IConfiguration configuration)
    {
        // Check if user already exists
        var email = configuration["UserSeed:Email"] 
            ?? throw new InvalidOperationException("User seed email not configured");
        
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            // User already exists, skip seeding
            return;
        }

        // 3. Create the initial user user
        var userResult = Participant.Create(
            Guid.NewGuid(),
            "Sandor Clegane The Hound",
            email);
        
        if (userResult.Failed)
            throw new InvalidOperationException("Failed to create user instance");

        // Get password from configuration
        var userPassword = configuration["UserSeed:Password"] 
            ?? throw new InvalidOperationException("User seed password not configured");

        var result = await userManager.CreateAsync(userResult.Value, userPassword);

        if (result.Succeeded)
        {
            // 4. Assign User role
            await userManager.AddToRoleAsync(userResult.Value, UserRoles.User);
            
            Console.WriteLine($"✅ Initial user account created: {email}");
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }
    }
}
