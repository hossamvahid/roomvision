using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Entities;

namespace roomvision.domain.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        public Task<Account?> GetByIdAsync(int id);
        public Task<Account?> GetByEmailAsync(string email);
        public Task AddAsync(Account account);
        public Task UpdateAsync(Account account);
        public Task DeleteByIdAsync(Account account);
    }
}