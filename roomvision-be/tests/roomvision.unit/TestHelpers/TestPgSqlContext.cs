using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using roomvision.infrastructure.Contexts;

namespace roomvision.unit.TestHelpers
{
   
    public class TestPgSqlContext : PgSqlContext
    {
        private readonly bool _shouldThrowOnSave;

        
        public TestPgSqlContext(DbContextOptions<PgSqlContext> options, bool shouldThrowOnSave = false) 
            : base(options)
        {
            _shouldThrowOnSave = shouldThrowOnSave;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_shouldThrowOnSave)
            {
                throw new DbUpdateException("Simulated database error");
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}