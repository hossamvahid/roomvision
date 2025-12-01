using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roomvision.infrastructure.Models;
public class FaceDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    [BsonRequired]
    public string? Id { get; set; }

    [BsonElement("PersonName")]
    [BsonRequired]
    public string? PersonName { get; set; }

    [BsonElement("Encoding")]
    [BsonRequired]
    public float[]? Encoding { get; set; }

}
