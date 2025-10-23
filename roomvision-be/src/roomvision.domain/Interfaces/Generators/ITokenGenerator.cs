using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Entities;

namespace roomvision.domain.Interfaces.Generators
{
    /// <summary>
    /// Interface for generating tokens.
    /// </summary>
    /// <remarks>
    /// This interface defines methods for generating tokens for various entities such as users and rooms.
    /// </remarks>
    public interface ITokenGenerator
    {
        /// <summary>
        /// Generates a user token.
        /// </summary>
        /// <remarks>
        /// This method creates a token for the given user account. 
        /// With the following claims included in the token:
        /// - User ID -> sub
        /// - Role (Account) -> role
        /// </remarks>
        /// <param name="account">The user account for which to generate the token.</param>
        /// <returns>The generated user token as a string.</returns>
        public string GenerateUserToken(Account account);

        /// <summary>
        /// Generates a room token.
        /// </summary>
        /// <remarks>
        /// This method creates a token for the given room. 
        /// With the following claims included in the token:
        /// - Room ID -> sub
        /// - Role (Room) -> role
        /// </remarks>
        /// <param name="room">The room for which to generate the token.</param>
        /// <returns>The generated room token as a string.</returns>
        public string GenerateRoomToken(Room room);
    }
}