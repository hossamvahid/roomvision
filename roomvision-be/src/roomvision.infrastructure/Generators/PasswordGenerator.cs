using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Interfaces.Generators;

namespace roomvision.infrastructure.Generators
{
    public class PasswordGenerator : IPasswordGenerator
    {
        public string GenerateHashedPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}