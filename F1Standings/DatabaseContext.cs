using System.Data.Common;
using System.Data.Entity;

namespace ConsoleApplication1
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Racer> Racers { get; set; }

        public DatabaseContext(DbConnection connection) : base(connection, true)
        {
            
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Racer>()
                .HasKey(r => r.RacerId);
        }
    }
}