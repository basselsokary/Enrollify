using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Infrastructure.Persistence.ReadContext;

internal sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly string _databaseName = "enrollify";

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("ReadDb"));
        _database = client.GetDatabase(_databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
        => _database.GetCollection<T>(name);
    
    public static void Configure()
    {
        BsonSerializer.RegisterSerializer(
            new GuidSerializer(GuidRepresentation.Standard));
    }
}