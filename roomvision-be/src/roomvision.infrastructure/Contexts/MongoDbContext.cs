using System;
using MongoDB.Driver;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Contexts;

public class MongoDbContext
{
    public IMongoCollection<FaceDbModel> Faces { get; }
    public MongoDbContext(IMongoDatabase database)
    {
        Faces = database.GetCollection<FaceDbModel>("Faces");
    }
}
