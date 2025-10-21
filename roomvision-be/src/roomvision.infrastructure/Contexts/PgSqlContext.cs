using Microsoft.EntityFrameworkCore;
using roomvision.infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roomvision.infrastructure.Contexts
{
    public class PgSqlContext : DbContext
    {
        public DbSet<AccountDbModel> Accounts { get; set; }
        public DbSet<RoomDbModel> Rooms { get; set; }
        
        public PgSqlContext(DbContextOptions options) : base(options) { }   

    }
}
