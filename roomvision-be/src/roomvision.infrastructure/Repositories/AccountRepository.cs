using Microsoft.EntityFrameworkCore;
using roomvision.application.Interfaces.Mappers;
using roomvision.application.Interfaces.Repositories;
using roomvision.domain.Entities;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roomvision.infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly PgSqlContext _context;
        private readonly IGenericMapper _mapper;

        public AccountRepository(PgSqlContext context, IGenericMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _context.Accounts.AnyAsync(x => x.Email == email);
        }

        public async Task<AccountEntity?> GetByEmailAsync(string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);

            if (account is null)
            {
                return null;
            }

            return _mapper.Map<Account,AccountEntity>(account);
        }

        public async Task AddAsync(AccountEntity account)
        {
            var addAccount = _mapper.Map<AccountEntity,Account>(account);

            await _context.Accounts.AddAsync(addAccount);
        }

        public void RemoveAsync(AccountEntity account)
        {
            var removeAccount = _mapper.Map<AccountEntity,Account>(account);

            _context.Accounts.Remove(removeAccount);
        }

    }
}
