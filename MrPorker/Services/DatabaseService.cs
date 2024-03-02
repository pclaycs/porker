﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MrPorker.Data;
using MrPorker.Data.Dtos;
using MrPorker.Data.Enums;
using MrPorker.Data.Models;
using MrPorker.Data.Models.SubModels;
using System.Text.Json;

namespace MrPorker.Services
{
    public class DatabaseService(IServiceProvider serviceProvider, IMapper mapper)
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

        public async Task<MeasurementDto?> GetXthMostRecentMeasurementAsync(int x, Competitor competitor)
        {
            if (x < 1) return null;

            // Get the current date and time
            var now = DateTime.Now;

            // Check if current time is before 4 AM and adjust the reference date accordingly
            var referenceDate = now.Hour < 4 ? now.Date.AddDays(-1) : now.Date;

            // Calculate the Unix timestamp range
            var endDate = referenceDate.AddHours(4).AddDays(-x + 1).ToUniversalTime();
            var startDate = endDate.AddDays(-1);

            long startTimestamp = ((DateTimeOffset)startDate).ToUnixTimeSeconds();
            long endTimestamp = ((DateTimeOffset)endDate).ToUnixTimeSeconds();

            return await WithDbContextAsync(async dbContext =>
            {
                if (competitor == Competitor.Addymer)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.AddymerMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Paul)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.Measurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Alex)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.AlexMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Eunora)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.EunoraMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Blu)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.BluMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());
                }

                return new MeasurementDto();
            });
        }

        public async Task<MeasurementDto?> GetStartingMeasurement(Competitor competitor)
        {
            return await WithDbContextAsync(async dbContext =>
            {
                if (competitor == Competitor.Addymer)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.AddymerMeasurements
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Paul)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.Measurements
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Alex)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.AlexMeasurements
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Eunora)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.EunoraMeasurements
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Blu)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.BluMeasurements
                        .FirstOrDefaultAsync());
                }

                return new MeasurementDto();
            });
        }

        public async Task AddMeasurementAsync(MeasurementDto measurementDto, Competitor competitor)
        {
            await WithDbContextAsync(async dbContext =>
            {
                if (competitor == Competitor.Addymer)
                {
                    var measurementModel = mapper.Map<AddymerMeasurementModel>(measurementDto);
                    dbContext.AddymerMeasurements.Add(measurementModel);
                }
                else if (competitor == Competitor.Paul)
                {
                    var measurementModel = mapper.Map<PaulMeasurementModel>(measurementDto);
                    dbContext.Measurements.Add(measurementModel);
                }
                else if (competitor == Competitor.Alex)
                {
                    var measurementModel = mapper.Map<AlexMeasurementModel>(measurementDto);
                    dbContext.AlexMeasurements.Add(measurementModel);
                }
                else if (competitor == Competitor.Eunora)
                {
                    var measurementModel = mapper.Map<EunoraMeasurementModel>(measurementDto);
                    dbContext.EunoraMeasurements.Add(measurementModel);
                }
                else if (competitor == Competitor.Blu)
                {
                    var measurementModel = mapper.Map<BluMeasurementModel>(measurementDto);
                    dbContext.BluMeasurements.Add(measurementModel);
                }

                await dbContext.SaveChangesAsync();

                return Task.CompletedTask;
            });
        }

        public async Task<int> GetDaysSinceFirstMeasurementAsync()
        {
            return await WithDbContextAsync(async dbContext =>
            {
                var result = mapper.Map<MeasurementDto>(await dbContext.Measurements
                    .OrderBy(m => m.Timestamp)
                    .FirstOrDefaultAsync());

                if (result != null)
                {
                    // Assuming result.Timestamp is a Unix timestamp in seconds
                    var timestampDateTime = DateTimeOffset.FromUnixTimeSeconds(result.Timestamp).DateTime;

                    // Calculate the number of days since the timestamp
                    return Convert.ToInt32(Math.Floor((DateTime.Now - timestampDateTime).TotalDays));
                }

                return -1;
            });
        }

        public async Task<string?> GetPhraseByIdAsync(int id)
        {
            return await WithDbContextAsync(async dbContext =>
            {
                var result = await dbContext.Phrases.FirstOrDefaultAsync(x => x.Id == id);
                return result?.Content;
            });
        }
    }
}
