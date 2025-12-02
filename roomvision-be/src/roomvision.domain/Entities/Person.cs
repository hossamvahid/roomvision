using System;

namespace roomvision.domain.Entities;

public class Person
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public DateOnly CreatedAt { get; set; }
}
