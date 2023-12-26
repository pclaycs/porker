using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrPorker.Configs;
using MrPorker.Data;
using MrPorker.Data.Models;
using MrPorker.Services;
using System.Text.Json;

namespace MrPorker
{
    public class Bot(BotDbContext context, DiscordSocketClient client, CommandHandler commandHandler, BotConfig config)
    {
        public async Task RunAsync()
        {
            await SeedDatabaseAsync();

            await client.LoginAsync(TokenType.Bot, config.BotToken);
            await client.StartAsync();

            // Initialize the CommandHandler
            await commandHandler.InitializeAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task SeedDatabaseAsync()
        {
            if (!context.Phrases.Any())
            {
                var filePath = "Assets/phrases.json";
                var jsonContent = await File.ReadAllTextAsync(filePath);
                var phrasesContent = JsonSerializer.Deserialize<IList<string>>(jsonContent);

                if (phrasesContent != null)
                {
                    var phrases = phrasesContent.Select(line => new PhraseModel { Content = line }).ToList();
                    context.Phrases.AddRange(phrases);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
