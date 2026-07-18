using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Infrastructure.Authentication;
using Domain.Entities.PaymentAggregate;
using Domain.Entities.EnrollmentAggregate;
using Domain.Entities.CourseAggregate;
using Infrastructure.Outbox;
using Infrastructure.Identity;

namespace Infrastructure.Persistence.WriteContext.Context;

internal class WriteDbContext(DbContextOptions<WriteDbContext> options)
    : IdentityDbContext<Participant, IdentityRole<Guid>, Guid>(options)
{
    private const string Schema = "enrollify";

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Customize table names
        builder.Entity<Participant>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

        // Apply all entity configurations from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);

        // Configure default schema
        builder.HasDefaultSchema(Schema);
    }
}
