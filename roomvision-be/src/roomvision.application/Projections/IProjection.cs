using System;

namespace roomvision.application.Projections;

public interface IProjection<T, R>
{
    Task<IReadOnlyList<R>> ProjectAsync(IReadOnlyList<T> rooms);
}
