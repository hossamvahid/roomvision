using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.application.Common;

namespace roomvision.application.Interfaces.Servicies
{
    public interface IAuthenticationService
    {
        public Task<Result<string>> UserAuthenticateAsync(string email, string password);
        public Task<Result<string>> RoomAuthenticateAsync(string roomName, string password);
    }
}