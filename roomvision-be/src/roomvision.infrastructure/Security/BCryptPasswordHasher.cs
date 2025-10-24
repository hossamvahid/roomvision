using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Interfaces.Security;

namespace roomvision.infrastructure.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string GenerateHashedPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyHashedPassword(string providedPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}