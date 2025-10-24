using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.domain.Interfaces.Security
{
    /// <summary>
    /// Interface for generating hashed passwords.
    /// </summary>
    /// <remarks>
    /// This interface defines methods for hashing passwords and verifying them.
    /// </remarks>
    public interface IPasswordHasher
    {

        /// <summary>
        /// Generates a hashed password.
        /// </summary>
        /// <remarks>
        /// This method takes a plain text password and returns its hashed version.
        /// </remarks>
        /// <param name="password">The plain text password to be hashed.</param>
        /// <returns>The hashed password as a string.</returns>
        public string GenerateHashedPassword(string password);

        /// <summary>
        /// Verifies a hashed password against a provided plain text password.
        /// </summary>
        /// <remarks>
        /// This method checks if the provided plain text password matches the hashed password.
        /// </remarks>
        /// <param name="providedPassword">The plain text password to verify.</param>
        /// <param name="hashedPassword">The hashed password to verify against.</param>
        /// <returns>True if the passwords match; otherwise, false.</returns>
        public bool VerifyHashedPassword(string providedPassword, string hashedPassword);
    }
}