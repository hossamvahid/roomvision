using roomvision.domain.Common;

namespace roomvision.application.Interfaces.Servicies.AccountServices
{
    public interface IDeleteAccountService
    {
        Task<Result> Execute(int accountId);
    }
}
