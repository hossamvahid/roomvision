using System;
using Microsoft.EntityFrameworkCore;
using roomvision.infrastructure.Contexts;

namespace roomvision.unit.TestHelpers
{
    public static class DatabaseTestHelper
    {
        
        public static DbContextOptions<PgSqlContext> CreateInMemoryContext()
        {
            return new DbContextOptionsBuilder<PgSqlContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options as DbContextOptions<PgSqlContext>;
        }
    }
}