using log4net;
using Microsoft.EntityFrameworkCore;
using roomvision.domain.Interfaces.Mappers;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Mappers;


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

            //Mappers 
            services.AddScoped<IGenericMapper, GenericMapper>();

        
            //Log
            services.AddSingleton(LogManager.GetLogger("SERVER"));

        }
    }
}
