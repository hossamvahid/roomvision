using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace roomvision.application.Utilities
{
    /// <summary>
    /// Utility class for password operations such as generation and verification.
    /// </summary>
    /// <remarks>
    /// This class provides methods for password generation and other related utilities.
    /// </remarks>
    public static class PasswordUtility
    {
        /// <summary>
        /// Generates a strong random password.
        /// </summary>
        /// <remarks>
        /// This method creates a 12-character password containing uppercase, lowercase, digits, 
        /// and special characters.
        /// </remarks>
        /// <returns>A randomly generated strong password.</returns>
        public static string GeneratePassword()
        {
            string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string digits = "0123456789";
            string specialChars = "!@#$%^&*()-+";
            string allChars = lowerCase + upperCase + digits + specialChars;
            int Length = 12;

            var password = new StringBuilder();
            password.Append(GetRandomChar(lowerCase));
            password.Append(GetRandomChar(upperCase));
            password.Append(GetRandomChar(digits));
            password.Append(GetRandomChar(specialChars));

            while (password.Length < Length)
            {
                password.Append(GetRandomChar(allChars));
            }

            return Shuffle(password.ToString());
        }

        /// <summary>
        /// Gets a random character from the specified string of possible characters.
        /// </summary>
        /// <param name="possibleChars">A string containing possible characters to choose from.</param>
        /// <returns>A randomly selected character from the possible characters.</returns>
        private static char GetRandomChar(string possibleChars)
        {
            var index = RandomNumberGenerator.GetInt32(possibleChars.Length);
            return possibleChars[index];
        }
        
        /// <summary>
        /// Shuffles the characters in the input string randomly.
        /// </summary>
        /// <param name="input">The string to shuffle.</param>
        /// <returns>A new string with the characters shuffled.</returns>
        private static string Shuffle(string input)
        {
            var array = input.ToCharArray();
            int n = array.Length;

            for (int i = 0; i < n; i++)
            {
                int random = RandomNumberGenerator.GetInt32(i, n);
                (array[i], array[random]) = (array[random], array[i]);
            }
            return new string(array);
        }
        

    }
}