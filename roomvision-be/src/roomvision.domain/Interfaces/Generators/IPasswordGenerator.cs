using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.domain.Interfaces.Generators
{
    /// <summary>
    /// Interface for generating hashed passwords.
    /// </summary>
    /// <remarks>
    /// This interface defines methods for generating hashed passwords using BCrypt.
    /// </remarks>
    public interface IPasswordGenerator
    {

        /// <summary>
        /// Generates a hashed password.
        /// </summary>
        /// <remarks>
        /// This method takes a plain text password and returns its hashed version using BCrypt.
        /// </remarks>
        /// <param name="password">The plain text password to be hashed.</param>
        /// <returns>The hashed password as a string.</returns>
        public string GenerateHashedPassword(string password);
    }
}