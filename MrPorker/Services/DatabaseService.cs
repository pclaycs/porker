using Microsoft.EntityFrameworkCore;
using MrPorker.Data;
using MrPorker.Data.Models;

namespace MrPorker.Services
{
    public class DatabaseService(BotDbContext context)
    {
        private readonly BotDbContext _context = context;

        public async Task<HoroscopeModel?> GetHoroscopeSignAsync(ulong userId)
        {
            return await _context.Horoscopes
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task SetHoroscopeSignAsync(ulong userId, int horoscopeSign)
        {
            var userHoroscope = await GetHoroscopeSignAsync(userId);
            if (userHoroscope == null)
            {
                userHoroscope = new HoroscopeModel { UserId = userId };
                _context.Horoscopes.Add(userHoroscope);
            }

            userHoroscope.Sign = horoscopeSign;
            await _context.SaveChangesAsync();
        }

    }
}
