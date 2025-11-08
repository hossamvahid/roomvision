using System;

namespace roomvision.domain.Entities;

public class Face
{
    public string? Id { get; set; }
    public string? PersonName { get; set; }
    public float[]? Encoding { get; set; }
}
