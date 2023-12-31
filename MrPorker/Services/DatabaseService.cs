﻿using Microsoft.EntityFrameworkCore;
using MrPorker.Data;
using MrPorker.Data.Models;
using System.Text.Json;

namespace MrPorker.Services
{
    public class DatabaseService(IServiceProvider serviceProvider)
    {
        private async Task<TResult> WithDbContextAsync<TResult>(Func<BotDbContext, Task<TResult>> action)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();
            return await action(dbContext);
        }

        public async Task SeedDatabaseAsync()
        {
            await WithDbContextAsync(async dbContext =>
            {
                if (!dbContext.Phrases.Any())
                {
                    var filePath = "Assets/phrases.json";
                    var jsonContent = await File.ReadAllTextAsync(filePath);
                    var phrasesContent = JsonSerializer.Deserialize<IList<string>>(jsonContent);

                    if (phrasesContent != null)
                    {
                        var phrases = phrasesContent.Select(line => new PhraseModel { Content = line }).ToList();
                        dbContext.Phrases.AddRange(phrases);
                        await dbContext.SaveChangesAsync();
                    }
                }

                return Task.CompletedTask;
            });
        }

        public async Task<HoroscopeModel?> GetHoroscopeSignAsync(ulong userId)
        {
            return await WithDbContextAsync(async dbContext =>
            {
                return await dbContext.Horoscopes.FirstOrDefaultAsync(u => u.UserId == userId);
            });
        }

        public async Task SetHoroscopeSignAsync(ulong userId, int horoscopeSign)
        {
            await WithDbContextAsync(async dbContext =>
            {
                var userHoroscope = await dbContext.Horoscopes.FirstOrDefaultAsync(u => u.UserId == userId);
                if (userHoroscope == null)
                {
                    userHoroscope = new HoroscopeModel { UserId = userId };
                    dbContext.Horoscopes.Add(userHoroscope);
                }

                userHoroscope.Sign = horoscopeSign;
                await dbContext.SaveChangesAsync();

                return Task.CompletedTask;
            });
        }
    }
}
