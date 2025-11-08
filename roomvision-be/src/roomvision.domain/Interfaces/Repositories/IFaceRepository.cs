using System;
using roomvision.domain.Entities;

namespace roomvision.domain.Interfaces.Repositories;

public interface IFaceRepository
{
    public Task<Face?> GetByIdAsync(string id);
    public Task AddAsync(Face face);
    public Task UpdateAsync(Face face);
    public Task DeleteAsync(Face face);
}
