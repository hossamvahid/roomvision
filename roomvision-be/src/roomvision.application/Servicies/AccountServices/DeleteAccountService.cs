using log4net;
using roomvision.application.Interfaces.Servicies.AccountServices;
using roomvision.domain.Common;
using roomvision.domain.Enums;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.AccountServices
{
    public class DeleteAccountService : IDeleteAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILog _log;

        public DeleteAccountService(IAccountRepository accountRepository, ILog log)
        {
            _accountRepository = accountRepository;
            _log = log;
        }

        public async Task<Result> Execute(int accountId)
        {
            _log.Info($"Deleting account with id: {accountId}");
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account is null)
            {
                _log.Warn($"Account with id {accountId} not found.");
                return Result.Failure("Account not found.", ErrorTypes.NotFound);
            }

            if (account.Role == Role.Admin)
            {
                _log.Warn($"Attempted to delete Admin account with id {accountId}.");
                return Result.Failure("Admin accounts cannot be deleted.", ErrorTypes.Validation);
            }

            await _accountRepository.DeleteByIdAsync(account);
            _log.Info($"Account with id {accountId} deleted successfully.");
            return Result.Success();
        }
    }
}
