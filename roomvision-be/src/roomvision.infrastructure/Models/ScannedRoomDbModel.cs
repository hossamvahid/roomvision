using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roomvision.infrastructure.Models;

public class ScannedRoomDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    [BsonRequired]
    public string? Id { get; set; }

    [BsonElement("RoomName")]
    [BsonRequired]
    public string? RoomName { get; set; }

    [BsonElement("IdentifiedFaces")]
    [BsonRequired]
    public List<string>? IdentifiedFaces { get; set; }

    [BsonElement("TotalFaces")]
    [BsonRequired]
    public int TotalFaces { get; set; }

    [BsonElement("ScannedAt")]
    [BsonRequired]
    public DateTime ScannedAt { get; set; }
}
