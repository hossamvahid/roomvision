using System;

namespace roomvision.application.Models;

public class AccountDetails
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}
