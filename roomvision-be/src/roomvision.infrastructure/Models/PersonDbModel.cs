using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roomvision.infrastructure.Models;
public class PersonDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    [BsonRequired]
    public string? Id { get; set; }

    [BsonElement("PersonName")]
    [BsonRequired]
    public string? Name { get; set; }

    [BsonElement("CreatedAt")]
    [BsonRequired]
    public DateOnly CreatedAt { get; set; }

}
