using Discord;
using Discord.WebSocket;
using MrPorker.Configs;
using MrPorker.Data;
using MrPorker.Services;

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
                var filePath = "";
            }
        }
    }
}
