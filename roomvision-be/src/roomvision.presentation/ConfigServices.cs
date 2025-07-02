using Microsoft.EntityFrameworkCore;
using roomvision.infrastructure.Contexts;

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
        }
    }
}
