using System;

namespace roomvision.application.Models;

public sealed class RoomWithScan
{
    public string? RoomName { get; set; }
    public int TotalFaces { get; set; }
    public DateTime ScannedAt { get; set; }
}
