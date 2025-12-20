using System.Security.Cryptography;
using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using roomvision.application.Interfaces.Servicies;
using roomvision.application.Interfaces.Servicies.AccountServices;
using roomvision.application.Interfaces.Servicies.PersonServices;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.application.Projections;
using roomvision.application.Servicies;
using roomvision.application.Servicies.AccountServices;
using roomvision.application.Servicies.PersonServices;
using roomvision.application.Servicies.RoomServices;
using roomvision.domain.Interfaces.FaceRecognition;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Mappers;
using roomvision.domain.Interfaces.Repositories;
using roomvision.domain.Interfaces.Security;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.FaceRecognition;
using roomvision.infrastructure.Generators;
using roomvision.infrastructure.Mappers;
using roomvision.infrastructure.Repositories;
using roomvision.infrastructure.Security;
using FaceRecognitionClient = FaceRecognitionGrpc.FaceRecognition.FaceRecognitionClient;

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

            services.AddDbContext<PgSqlContext>(opt => opt.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_DATABASE")));

            var mongoClient = new MongoDB.Driver.MongoClient(Environment.GetEnvironmentVariable("MONGODB_CONNECTION"));
            var mongoDatabase = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGODB_DATABASE"));
            services.AddSingleton(mongoDatabase);
            services.AddSingleton<MongoDbContext>();

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
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IMailGenerator, MailGenerator>();

            //Face Recognition
            services.AddScoped<FaceRecognitionClient>(provider =>
            {
                var channel = Grpc.Net.Client.GrpcChannel.ForAddress(Environment.GetEnvironmentVariable("FACE_RECOGNITION_GRPC_URL")!);
                return new FaceRecognitionClient(channel);
            });
            services.AddScoped<IFaceRecognition, GrpcFaceRecognition>();

            //Repositories`
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IScannedRoomRepository, ScannedRoomRepository>();

            //Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICreateAccountService, CreateAccountService>();
            services.AddScoped<IResetAccountPasswordService, ResetAccountPasswordService>();
            services.AddScoped<ICreatePersonService, CreatePersonService>();
            services.AddScoped<IScanRoomService, ScanRoomService>();
            services.AddScoped<IScanResultService, ScanResultService>();
            services.AddScoped<IGetRooms, GetRooms>();

            //Log
            services.AddSingleton(LogManager.GetLogger("SERVER"));

            //Projections
            services.AddScoped<RoomWithScanProjection>();

        }
    }
}
