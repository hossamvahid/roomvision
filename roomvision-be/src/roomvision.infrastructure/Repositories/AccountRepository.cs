using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.EntityFrameworkCore;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Mappers;
using roomvision.domain.Interfaces.Repositories;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IGenericMapper _mapper;
        private readonly PgSqlContext _context;
        
        public AccountRepository(IGenericMapper mapper, PgSqlContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            if (account is null)
            {
                return null;
            }
            return _mapper.Map<AccountDbModel, Account>(account);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            if (account is null)
            {
                return null;
            }
            return _mapper.Map<AccountDbModel, Account>(account);
        }

        public async Task AddAsync(Account account)
        {
            var mappedAccountToAdd = _mapper.Map<Account, AccountDbModel>(account);
            await  _context.AddAsync(mappedAccountToAdd);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            var mappedAccountToUpdate = _mapper.Map<Account, AccountDbModel>(account);
            _context.Update(mappedAccountToUpdate);
            await _context.SaveChangesAsync();            
        }
        
        public async Task DeleteByIdAsync(Account account)
        {
            var accountToDelete = _mapper.Map<Account, AccountDbModel>(account);
            _context.Remove(accountToDelete);
            await _context.SaveChangesAsync();
        }
    }
}