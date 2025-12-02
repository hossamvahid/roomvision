using System;
using roomvision.domain.Entities;

namespace roomvision.domain.Interfaces.Repositories;

public interface IPersonRepository
{
    public Task<Person?> GetByIdAsync(string id);
    public Task<Person?> GetByNameAsync(string name);
    public Task AddAsync(Person face);
    public Task UpdateAsync(Person face);
    public Task DeleteAsync(Person face);
}
