using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MrPorker.Configs;
using MrPorker.Data;
using MrPorker.Data.Dtos;
using MrPorker.Data.Enums;
using MrPorker.Data.Models;
using MrPorker.Data.Models.SubModels;
using System.Text.Json;

namespace MrPorker.Services
{
    public class DatabaseService(IServiceProvider serviceProvider, IMapper mapper, BotConfig botConfig)
    {
        private async Task<TResult> WithDbContextAsync<TResult>(Func<BotDbContext, Task<TResult>> action)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();
            return await action(dbContext);
        }

        public async Task ApplyMigrations()
        {
            await WithDbContextAsync(async dbContext =>
            {
                dbContext.Database.Migrate();
                return Task.CompletedTask;
            });
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

        public async Task RecalculateElo()
        {
            await WithDbContextAsync(async dbContext =>
            {
                foreach (var measurement in dbContext.Measurements)
                {
                    var dto = mapper.Map<MeasurementDto>(measurement);
                    dto.Height = 177;
                    dto.Age = botConfig.GetMeasurementThresholdConfigByCompetitor(Competitor.Paul).GetAge();

                    measurement.Strength = CharacterRanking.CalculateStrengthScore(dto);
                    measurement.Endurance = CharacterRanking.CalculateEnduranceScore(dto);
                    measurement.Agility = CharacterRanking.CalculateAgilityScore(dto);
                    measurement.Overall = CharacterRanking.CalculateOverallScore(dto, measurement.Strength, measurement.Endurance, measurement.Agility);
                }

                foreach (var measurement in dbContext.AddymerMeasurements)
                {
                    var dto = mapper.Map<MeasurementDto>(measurement);
                    dto.Height = 175;
                    dto.Age = botConfig.GetMeasurementThresholdConfigByCompetitor(Competitor.Addymer).GetAge();

                    measurement.Strength = CharacterRanking.CalculateStrengthScore(dto);
                    measurement.Endurance = CharacterRanking.CalculateEnduranceScore(dto);
                    measurement.Agility = CharacterRanking.CalculateAgilityScore(dto);
                    measurement.Overall = CharacterRanking.CalculateOverallScore(dto, measurement.Strength, measurement.Endurance, measurement.Agility);
                }

                foreach (var measurement in dbContext.AlexMeasurements)
                {
                    var dto = mapper.Map<MeasurementDto>(measurement);
                    dto.Height = 192;
                    dto.Age = botConfig.GetMeasurementThresholdConfigByCompetitor(Competitor.Alex).GetAge();

                    measurement.Strength = CharacterRanking.CalculateStrengthScore(dto);
                    measurement.Endurance = CharacterRanking.CalculateEnduranceScore(dto);
                    measurement.Agility = CharacterRanking.CalculateAgilityScore(dto);
                    measurement.Overall = CharacterRanking.CalculateOverallScore(dto, measurement.Strength, measurement.Endurance, measurement.Agility);
                }

                foreach (var measurement in dbContext.EunoraMeasurements)
                {
                    var dto = mapper.Map<MeasurementDto>(measurement);
                    dto.Height = 190;
                    dto.Age = botConfig.GetMeasurementThresholdConfigByCompetitor(Competitor.Eunora).GetAge();

                    measurement.Strength = CharacterRanking.CalculateStrengthScore(dto);
                    measurement.Endurance = CharacterRanking.CalculateEnduranceScore(dto);
                    measurement.Agility = CharacterRanking.CalculateAgilityScore(dto);
                    measurement.Overall = CharacterRanking.CalculateOverallScore(dto, measurement.Strength, measurement.Endurance, measurement.Agility);
                }

                foreach (var measurement in dbContext.BluMeasurements)
                {
                    var dto = mapper.Map<MeasurementDto>(measurement);
                    dto.Height = 175;
                    dto.Age = botConfig.GetMeasurementThresholdConfigByCompetitor(Competitor.Blu).GetAge();

                    measurement.Strength = CharacterRanking.CalculateStrengthScore(dto);
                    measurement.Endurance = CharacterRanking.CalculateEnduranceScore(dto);
                    measurement.Agility = CharacterRanking.CalculateAgilityScore(dto);
                    measurement.Overall = CharacterRanking.CalculateOverallScore(dto, measurement.Strength, measurement.Endurance, measurement.Agility);
                }

                foreach (var measurement in dbContext.BraydenMeasurements)
                {
                    var dto = mapper.Map<MeasurementDto>(measurement);
                    dto.Height = 178;
                    dto.Age = botConfig.GetMeasurementThresholdConfigByCompetitor(Competitor.Brayden).GetAge();

                    measurement.Strength = CharacterRanking.CalculateStrengthScore(dto);
                    measurement.Endurance = CharacterRanking.CalculateEnduranceScore(dto);
                    measurement.Agility = CharacterRanking.CalculateAgilityScore(dto);
                    measurement.Overall = CharacterRanking.CalculateOverallScore(dto, measurement.Strength, measurement.Endurance, measurement.Agility);
                }

                foreach (var measurement in dbContext.CbriMeasurements)
                {
                    var dto = mapper.Map<MeasurementDto>(measurement);
                    dto.Height = 188;
                    dto.Age = botConfig.GetMeasurementThresholdConfigByCompetitor(Competitor.Cbri).GetAge();

                    measurement.Strength = CharacterRanking.CalculateStrengthScore(dto);
                    measurement.Endurance = CharacterRanking.CalculateEnduranceScore(dto);
                    measurement.Agility = CharacterRanking.CalculateAgilityScore(dto);
                    measurement.Overall = CharacterRanking.CalculateOverallScore(dto, measurement.Strength, measurement.Endurance, measurement.Agility);
                }

                await dbContext.SaveChangesAsync();

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

        public async Task<MeasurementDto?> GetLatestMeasurementAsync(Competitor competitor)
        {
            return await WithDbContextAsync(async dbContext =>
            {
                if (competitor == Competitor.Addymer)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.AddymerMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Paul)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.Measurements
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Alex)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.AlexMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Eunora)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.EunoraMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Blu)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.BluMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Brayden)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.BraydenMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Cbri)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.CbriMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefaultAsync());
                }

                return new MeasurementDto();
            });
        }

        public async Task<MeasurementDto?> GetSecondLatestMeasurementAsync(Competitor competitor)
        {
            return await WithDbContextAsync(async dbContext =>
            {
                if (competitor == Competitor.Addymer)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.AddymerMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .Skip(1)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Paul)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.Measurements
                        .OrderByDescending(m => m.Timestamp)
                        .Skip(1)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Alex)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.AlexMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .Skip(1)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Eunora)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.EunoraMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .Skip(1)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Blu)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.BluMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .Skip(1)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Brayden)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.BraydenMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .Skip(1)
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Cbri)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.CbriMeasurements
                        .OrderByDescending(m => m.Timestamp)
                        .Skip(1)
                        .FirstOrDefaultAsync());
                }

                return new MeasurementDto();
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
                    var result = mapper.Map<MeasurementDto>(await dbContext.AddymerMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());

                    if (result == null && x == 1)
                        result = await GetSecondLatestMeasurementAsync(competitor);

                    return result;
                }
                else if (competitor == Competitor.Paul)
                {
                    var result = mapper.Map<MeasurementDto>(await dbContext.Measurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());

                    if (result == null && x == 1)
                        result = await GetSecondLatestMeasurementAsync(competitor);

                    return result;
                }
                else if (competitor == Competitor.Alex)
                {
                    var result = mapper.Map<MeasurementDto>(await dbContext.AlexMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());

                    if (result == null && x == 1)
                        result = await GetSecondLatestMeasurementAsync(competitor);

                    return result;
                }
                else if (competitor == Competitor.Eunora)
                {
                    var result = mapper.Map<MeasurementDto>(await dbContext.EunoraMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());

                    if (result == null && x == 1)
                        result = await GetSecondLatestMeasurementAsync(competitor);

                    return result;
                }
                else if (competitor == Competitor.Blu)
                {
                    var result = mapper.Map<MeasurementDto>(await dbContext.BluMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());

                    if (result == null && x == 1)
                        result = await GetSecondLatestMeasurementAsync(competitor);

                    return result;
                }
                else if (competitor == Competitor.Brayden)
                {
                    var result = mapper.Map<MeasurementDto>(await dbContext.BraydenMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());

                    if (result == null && x == 1)
                        result = await GetSecondLatestMeasurementAsync(competitor);

                    return result;
                }
                else if (competitor == Competitor.Cbri)
                {
                    var result = mapper.Map<MeasurementDto>(await dbContext.CbriMeasurements
                        .Where(m => m.Timestamp >= startTimestamp && m.Timestamp < endTimestamp)
                        .FirstOrDefaultAsync());

                    if (result == null && x == 1)
                        result = await GetSecondLatestMeasurementAsync(competitor);

                    return result;
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
                else if (competitor == Competitor.Brayden)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.BraydenMeasurements
                        .FirstOrDefaultAsync());
                }
                else if (competitor == Competitor.Cbri)
                {
                    return mapper.Map<MeasurementDto>(await dbContext.CbriMeasurements
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
                else if (competitor == Competitor.Brayden)
                {
                    var measurementModel = mapper.Map<BraydenMeasurementModel>(measurementDto);
                    dbContext.BraydenMeasurements.Add(measurementModel);
                }
                else if (competitor == Competitor.Cbri)
                {
                    var measurementModel = mapper.Map<CbriMeasurementModel>(measurementDto);
                    dbContext.CbriMeasurements.Add(measurementModel);
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
                    return Convert.ToInt32(Math.Floor((DateTime.Now - timestampDateTime).TotalDays)) - 76;
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
