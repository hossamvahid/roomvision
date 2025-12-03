using System;

namespace roomvision.presentation.Response.RoomResponse;

public class ScanResult
{
    public List<string>? IdentifiedFaces { get; set; }
    public DateTime ScannedAt { get; set; }
}
