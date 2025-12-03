using System;

namespace roomvision.domain.Entities;

public class ScannedRoom
{
    public string? Id { get; set; }
    public string? RoomName { get; set; }
    public List<string>? IdentifiedFaces { get; set; }
    public int TotalFaces { get; set; }
    public DateTime ScannedAt { get; set; }
}
