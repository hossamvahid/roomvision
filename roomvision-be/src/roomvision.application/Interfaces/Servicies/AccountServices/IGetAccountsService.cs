using System;
using roomvision.application.Models;
using roomvision.domain.Common;

namespace roomvision.application.Interfaces.Servicies.AccountServices;

public interface IGetAccountsService
{
    Task<Result<Paginated<AccountDetails>>> Execute(int page, int pageSize = 6, int? excludeAccountId = null);
}
