using System;
using roomvision.domain.Common;

namespace roomvision.application.Interfaces.Servicies.AccountServices;

public interface IGetAccountNameService
{
    Task<Result<string>> Execute(int accountId);
}
