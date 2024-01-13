using Microsoft.EntityFrameworkCore;
using MrPorker.Data.Models;
using MrPorker.Data.Models.SubModels;

namespace MrPorker.Data
{
    public class BotDbContext(DbContextOptions<BotDbContext> options) : DbContext(options)
    {
        public DbSet<HoroscopeModel> Horoscopes { get; set; }
        public DbSet<PaulMeasurementModel> Measurements { get; set; }
        public DbSet<AddymerMeasurementModel> AddymerMeasurements { get; set; }
        public DbSet<AlexMeasurementModel> AlexMeasurements { get; set; }
        public DbSet<PhraseModel> Phrases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoroscopeModel>()
                .HasKey(e => e.UserId);
        }
    }
}
