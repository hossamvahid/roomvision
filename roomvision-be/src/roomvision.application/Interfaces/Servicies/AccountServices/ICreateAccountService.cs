using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Common;
using roomvision.domain.Enums;
namespace roomvision.application.Interfaces.Servicies.AccountServices
{
    public interface ICreateAccountService
    {
        public Task<Result> Execute(string email, string username, Role role);
    }
}