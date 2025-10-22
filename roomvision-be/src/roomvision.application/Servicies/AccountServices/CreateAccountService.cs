using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using roomvision.application.Common;
using roomvision.application.Interfaces.Servicies.AccountServices;
using roomvision.application.Utilities;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.AccountServices
{
    public class CreateAccountService : ICreateAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILog _log;
        private readonly IMailGenerator _mailGenerator;
        private readonly IPasswordGenerator _passwordGenerator;

        public CreateAccountService(
            IAccountRepository accountRepository,
            ILog log,
            IMailGenerator mailGenerator,
            IPasswordGenerator passwordGenerator)
        {
            _accountRepository = accountRepository;
            _log = log;
            _mailGenerator = mailGenerator;
            _passwordGenerator = passwordGenerator;
        }

        public async Task<Result> Execute(string email, string name)
        {
            _log.Info($"Creating account for email: {email}, name: {name}");
            var account = await _accountRepository.GetByEmailAsync(email);

            if (account is not null)
            {
                _log.Warn($"Account with email {email} already exists.");
                return Result.Failure("Account with this email already exists.", ErrorTypes.Conflict);
            }

            _log.Info("Generating password and creating account.");
            var password = PasswordUtility.GeneratePassword();

            var newAccount = new Account
            {
                Email = email,
                Name = name,
                Password = _passwordGenerator.GenerateHashedPassword(password)
            };
            
            _log.Info("Saving account and sending welcome email.");
            await _accountRepository.AddAsync(newAccount);
            await _mailGenerator.WelcomeEmailAsync(email, password);
            
            _log.Info("Account created successfully.");
            return Result.Success();
        }
    }
}