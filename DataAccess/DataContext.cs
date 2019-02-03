using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=SENDOH-PC\SQLEXPRESS;Database=Robot;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        public virtual DbSet<Sistemas> Sistemas { get; set; }
        public virtual DbSet<SodimacCLURL> SodimacCLURL { get; set; }
        public virtual DbSet<SodimacCLProducto> SodimacCLProducto { get; set; }
        public virtual DbSet<FalabellaProducto> FalabellaProducto { get; set; }
        public virtual DbSet<FalabellaURL> FalabellaURL { get; set; }
        public virtual DbSet<LogErrores> LogErrores { get; set; }
        public virtual DbSet<Stats> Stats { get; set; }
    }
}
