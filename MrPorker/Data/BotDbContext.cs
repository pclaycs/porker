using Microsoft.EntityFrameworkCore;
using MrPorker.Data.Models;

namespace MrPorker.Data
{
    public class BotDbContext : DbContext
    {
        public DbSet<HoroscopeModel> Horoscopes { get; set; }
        public DbSet<HoroscopeModel> Phrases { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=botdata.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoroscopeModel>()
                .HasKey(e => e.UserId);
        }

    }
}
