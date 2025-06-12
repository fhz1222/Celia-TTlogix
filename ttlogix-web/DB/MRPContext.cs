using Microsoft.EntityFrameworkCore;
using TT.Core.Entities.MRP;

namespace TT.DB
{
    public class MRPContext : DbContext
    {
        public DbSet<MRPInventory> MRPInventory { get; set; }
        public DbSet<MRPItemMaster> MRPItemMasters { get; set; }

        public MRPContext(DbContextOptions<MRPContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MRPInventory>().HasKey(c => new { c.FactoryID, c.SupplierID, c.ProductCode });
            modelBuilder.Entity<MRPItemMaster>().HasKey(c => new { c.FactoryID, c.SupplierID, c.ProductCode });
        }
    }
}
