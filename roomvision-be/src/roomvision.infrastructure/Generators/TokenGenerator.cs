using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Generators;

namespace roomvision.infrastructure.Generators
{
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateUserToken(Account account)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(ClaimTypes.Role, "Account"),
            };

            RSA privateKey = RSA.Create();
            var privateKeyPem = File.ReadAllText("private.pem");
            privateKey.ImportFromPem(privateKeyPem.ToCharArray());

            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(privateKey),
                SecurityAlgorithms.RsaSha256
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public string GenerateRoomToken(Room room)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, room.Id.ToString()),
                new Claim(ClaimTypes.Role, "Room"),
            };

            RSA privateKey = RSA.Create();
            var privateKeyPem = File.ReadAllText("private.pem");
            privateKey.ImportFromPem(privateKeyPem.ToCharArray());

            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(privateKey),
                SecurityAlgorithms.RsaSha256
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}