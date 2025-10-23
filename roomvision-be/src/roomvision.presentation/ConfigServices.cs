using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using roomvision.application.Interfaces.Servicies;
using roomvision.application.Interfaces.Servicies.AccountServices;
using roomvision.application.Servicies;
using roomvision.application.Servicies.AccountServices;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Mappers;
using roomvision.domain.Interfaces.Repositories;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Generators;
using roomvision.infrastructure.Mappers;
using roomvision.infrastructure.Repositories;


namespace roomvision.presentation
{
    public static class ConfigServices
    {
        public static void AddConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddDbContext<PgSqlContext>(opt => opt.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE")));


            RSA publicKey = RSA.Create();
            var publicKeyPem = File.ReadAllText("public.pem");
            publicKey.ImportFromPem(publicKeyPem.ToCharArray());

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new RsaSecurityKey(publicKey),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            //Mappers 
            services.AddScoped<IGenericMapper, GenericMapper>();

            //Generators
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordGenerator, PasswordGenerator>();
            services.AddScoped<IMailGenerator, MailGenerator>();

            //Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();

            //Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICreateAccountService, CreateAccountService>();
            services.AddScoped<IResetAccountPasswordService, ResetAccountPasswordService>();
        
            //Log
            services.AddSingleton(LogManager.GetLogger("SERVER"));

        }
    }
}
