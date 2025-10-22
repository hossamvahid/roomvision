using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace roomvision.application.Utilities
{
    public static class PasswordUtility
    {
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

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


        private static char GetRandomChar(string possibleChars)
        {
            var index = RandomNumberGenerator.GetInt32(possibleChars.Length);
            return possibleChars[index];
        }

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