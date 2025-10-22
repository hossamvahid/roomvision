using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.application.Common;

namespace roomvision.application.Interfaces.Servicies.AccountServices
{
    public interface ICreateAccountService
    {
        public Task<Result> Execute(string email, string username);
    }
}