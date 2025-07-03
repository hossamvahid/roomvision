using Microsoft.EntityFrameworkCore;
using roomvision.application.Interfaces.Mappers;
using roomvision.application.Interfaces.Repositories;
using roomvision.infrastructure.Contexts;
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

            services.AddScoped<IGenericMapper, GenericMapper>();
            services.AddScoped<IAccountRepository,AccountRepository>();
            services.AddScoped<IDapi, Dapi>();
        }
    }
}
