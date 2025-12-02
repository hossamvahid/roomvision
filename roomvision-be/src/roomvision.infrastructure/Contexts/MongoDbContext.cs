using System;
using MongoDB.Driver;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Contexts;

public class MongoDbContext
{
    public IMongoCollection<PersonDbModel> Persons { get; }
    public MongoDbContext(IMongoDatabase database)
    {
        Persons = database.GetCollection<PersonDbModel>("Persons");
    }
}
