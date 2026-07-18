using Application.Common.ReadModels;
using Infrastructure.Persistence.ReadContext;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Seeding;

internal static class MongoDbInitializer
{
    public static async Task InitializeAsync(MongoDbContext context)
    {
        await CreateCourseIndexesAsync(context);
        await CreateEnrollmentIndexesAsync(context);
        await CreateUserIndexesAsync(context);
        await CreatePaymentIndexesAsync(context);
    }

    private static async Task CreateCourseIndexesAsync(MongoDbContext context)
    {
        var collection = context.GetCollection<CourseDocument>("courses");

        var indexes = new[]
        {
            new CreateIndexModel<CourseDocument>(
                Builders<CourseDocument>.IndexKeys.Ascending(c => c.IsFree)),

            new CreateIndexModel<CourseDocument>(
                Builders<CourseDocument>.IndexKeys.Text(c => c.Title))
        };

        await collection.Indexes.CreateManyAsync(indexes);
    }

    private static async Task CreateEnrollmentIndexesAsync(MongoDbContext context)
    {
        var collection = context.GetCollection<UserEnrollmentDocument>("user_enrollments");

        var index = new CreateIndexModel<UserEnrollmentDocument>(
            Builders<UserEnrollmentDocument>.IndexKeys.Ascending(e => e.UserId));

        await collection.Indexes.CreateOneAsync(index);
    }

    private static async Task CreateUserIndexesAsync(MongoDbContext context)
    {
        var collection = context.GetCollection<UserDocument>("users");

        var index = new CreateIndexModel<UserDocument>(
            Builders<UserDocument>.IndexKeys.Ascending(u => u.Email));

        await collection.Indexes.CreateOneAsync(index);
    }

    private static async Task CreatePaymentIndexesAsync(MongoDbContext context)
    {
        var collection = context.GetCollection<PaymentDocument>("payments");

        var index = new CreateIndexModel<PaymentDocument>(
            Builders<PaymentDocument>.IndexKeys.Ascending(p => p.UserId));

        await collection.Indexes.CreateOneAsync(index);
    }
}