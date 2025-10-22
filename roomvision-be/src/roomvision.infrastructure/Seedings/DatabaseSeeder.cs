using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Seedings
{
    public static class DatabaseSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider;
            var context = service.GetRequiredService<PgSqlContext>();
            
            if(context!.Accounts.Any(u => u.Email == Environment.GetEnvironmentVariable("ADMIN_EMAIL")) is false)
            {
                context.Accounts.Add(new AccountDbModel
                {
                    Name = Environment.GetEnvironmentVariable("ADMIN_NAME"),
                    Email = Environment.GetEnvironmentVariable("ADMIN_EMAIL"),
                    Password = BCrypt.Net.BCrypt.HashPassword(Environment.GetEnvironmentVariable("ADMIN_PASSWORD")!)
                });

                context.SaveChanges();
            }
                
        }
    }
}