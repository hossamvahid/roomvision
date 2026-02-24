using System;
using log4net;
using roomvision.application.Interfaces.Servicies.AccountServices;
using roomvision.application.Models;
using roomvision.application.Projections;
using roomvision.domain.Common;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.AccountServices;

public class GetAccountsService : IGetAccountsService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IProjection<Account, AccountDetails> _projection;
    private readonly ILog _log;

    public GetAccountsService(IAccountRepository accountRepository, IProjection<Account, AccountDetails> projection, ILog log)
    {
        _accountRepository = accountRepository;
        _projection = projection;
        _log = log;
    }

    public async Task<Result<Paginated<AccountDetails>>> Execute(int page, int pageSize = 6, int? excludeAccountId = null)
    {
        _log.Info($"Getting accounts from page: {page} with the size: {pageSize}");

        var accounts = await _accountRepository.GetAccountsPaginatedAsync(page, pageSize);

        if (excludeAccountId.HasValue)
        {
            accounts = accounts.Where(a => a.Id != excludeAccountId.Value).ToList();
        }

        _log.Info("Getting the total ammount of accounts");
        var totalItems = await _accountRepository.CountAsync();

        if (excludeAccountId.HasValue)
        {
            totalItems--;
        }

        _log.Info("Paginating accounts");
        var accountsDetails = await _projection.ProjectAsync(accounts);
        var paginatedResult = Paginated<AccountDetails>.Create(accountsDetails, totalItems, pageSize);

        return Result<Paginated<AccountDetails>>.Success(paginatedResult);
    }
}
