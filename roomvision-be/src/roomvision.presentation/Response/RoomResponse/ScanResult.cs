using System;

namespace roomvision.presentation.Response.RoomResponse;

public class ScanResult
{
    public List<string>? IdentifiedFaces { get; set; }
    public int TotalFaces { get; set; }
    public int TotalUnknown { get; set; }
    public DateTime ScannedAt { get; set; }
}
