using Microsoft.EntityFrameworkCore;
using MrPorker.Data.Models;

namespace MrPorker.Data
{
    public class BotDbContext(DbContextOptions<BotDbContext> options) : DbContext(options)
    {
        public DbSet<HoroscopeModel> Horoscopes { get; set; }
        public DbSet<MeasurementModel> Measurements { get; set; }
        public DbSet<PhraseModel> Phrases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoroscopeModel>()
                .HasKey(e => e.UserId);
        }
    }
}
