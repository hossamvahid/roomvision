using roomvision.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roomvision.application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        public Task<bool> EmailExistsAsync(string email);
        public Task<AccountEntity?> GetByEmailAsync(string email);
        public Task AddAsync(AccountEntity account);
        public void RemoveAsync(AccountEntity account);

    }
}
