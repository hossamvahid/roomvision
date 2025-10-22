using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Entities;

namespace roomvision.domain.Interfaces.Generators
{
    public interface ITokenGenerator
    {
        public string GenerateUserToken(Account account);
        public string GenerateRoomToken(Room room);
    }
}