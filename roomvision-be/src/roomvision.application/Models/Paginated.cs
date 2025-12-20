using System;

namespace roomvision.application.Models;

public class Paginated<T>
{
    public IReadOnlyList<T>? items { get; set; }
    public int totalPages { get; set; }

    private Paginated(IReadOnlyList<T> items, int totalItems, int pageSize)
    {
        this.items = items;
        totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }

    public static Paginated<T> Create(IReadOnlyList<T> items, int totalItems, int pageSize = 6)
    {
        return new Paginated<T>(items, totalItems, pageSize);
    }
}
