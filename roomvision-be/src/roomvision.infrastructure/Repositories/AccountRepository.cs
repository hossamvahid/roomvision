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

        private readonly ILog _log;

        public AccountRepository(IGenericMapper mapper, PgSqlContext context, ILog log)
        {
            _mapper = mapper;
            _context = context;
            _log = log;
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            if (account is null)
            {
                _log.Info($"Account with id {id} not found.");
                return null;
            }
            _log.Info($"Account with id {id} retrieved successfully.");
            return _mapper.Map<AccountDbModel, Account>(account);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            if (account is null)
            {
                _log.Info($"Account with email {email} not found.");
                return null;
            }
            _log.Info($"Account with email {email} retrieved successfully.");
            return _mapper.Map<AccountDbModel, Account>(account);
        }

        public async Task<bool> AddAsync(Account account)
        {
            var mappedAccount = _mapper.Map<Account, AccountDbModel>(account);

            try
            {
                _log.Info($"Adding account with email {account.Email}.");
                await _context.AddAsync(mappedAccount);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Account account)
        {
            var mappedAccountToUpdate = _mapper.Map<Account, AccountDbModel>(account);

            try
            {
                _log.Info($"Updating account with id {account.Id}.");
                _context.Update(mappedAccountToUpdate);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }
        
        public async Task<bool> DeleteByIdAsync(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            if(account is null)
            {
                _log.Info($"Account with id {id} not found.");
                return false;
            }

            try
            {
                _log.Info($"Deleting account with id {id}.");
                _context.Remove(account);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }
    }
}