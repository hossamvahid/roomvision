namespace roomvision.domain.ValueObjects;

public record RoomScanResult(List<string> IdentifiedFaces, int TotalFaces);
