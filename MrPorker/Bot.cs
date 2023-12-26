using Discord;
using Discord.WebSocket;
using MrPorker.Configs;
using MrPorker.Services;

namespace MrPorker
{
    public class Bot(DiscordSocketClient client, CommandHandler commandHandler, BotConfig config)
    {
        public async Task RunAsync()
        {
            await client.LoginAsync(TokenType.Bot, config.BotToken);
            await client.StartAsync();

            // Initialize the CommandHandler
            await commandHandler.InitializeAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}
