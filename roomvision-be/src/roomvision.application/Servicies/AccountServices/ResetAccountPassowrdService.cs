using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using roomvision.application.Common;
using roomvision.application.Interfaces.Servicies.AccountServices;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Repositories;
using roomvision.domain.Interfaces.Security;

namespace roomvision.application.Servicies.AccountServices
{
    public class ResetAccountPasswordService : IResetAccountPasswordService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILog _log;
        
        public ResetAccountPasswordService(
            IAccountRepository accountRepository,
            IPasswordHasher passwordHasher,
            ILog log)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _log = log;
        }
        public async Task<Result> Execute(int id, string newPassword)
        {
            _log.Info($"Initiating password reset for account id {id}.");
            var account = await _accountRepository.GetByIdAsync(id);
            
            if (account is null)
            {
                _log.Warn($"Account with id {id} not found for password reset.");
                return Result.Failure("Account not found.", ErrorTypes.NotFound);
            }

            var hashedNewPassword = _passwordHasher.GenerateHashedPassword(newPassword);
            account.Password = hashedNewPassword;

            await _accountRepository.UpdateAsync(account);
            _log.Info($"Password reset successful for account id {id}.");
            return Result.Success();
        }
    }
}