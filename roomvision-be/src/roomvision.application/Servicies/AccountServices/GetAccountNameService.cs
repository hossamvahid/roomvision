using System;
using roomvision.application.Interfaces.Servicies.AccountServices;
using roomvision.domain.Common;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.AccountServices;

public class GetAccountNameService : IGetAccountNameService
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountNameService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<string>> Execute(int accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);

        if (account is null)
        {
            return Result<string>.Failure("Account not found.", ErrorTypes.NotFound);
        }

        return Result<string>.Success(account.Name!);
    }

}
