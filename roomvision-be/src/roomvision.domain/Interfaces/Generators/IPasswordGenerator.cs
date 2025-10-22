using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.domain.Interfaces.Generators
{
    public interface IPasswordGenerator
    {
        public string GenerateHashedPassword(string password);
    }
}