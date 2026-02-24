using System;
using roomvision.application.Models;
using roomvision.domain.Entities;

namespace roomvision.application.Projections;

public class AccountDetailsProjection : IProjection<Account, AccountDetails>
{
    public Task<IReadOnlyList<AccountDetails>> ProjectAsync(IReadOnlyList<Account> accounts)
    {
        var result = new List<AccountDetails>(accounts.Count);

        foreach (var account in accounts)
        {
            result.Add(new AccountDetails
            {
                Id = account.Id,
                Name = account.Name!,
                Email = account.Email!
            });
        }

        return Task.FromResult((IReadOnlyList<AccountDetails>)result);
    }

}
