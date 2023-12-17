using Discord;
using Discord.WebSocket;
using MrPorker.Configs;
using MrPorker.Services;

namespace MrPorker
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly BotConfig _config;

        public Bot(DiscordSocketClient client, CommandHandler commandHandler, BotConfig config)
        {
            _client = client;
            _config = config;
            _commandHandler = commandHandler;
        }

        public async Task RunAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _config.BotToken);
            await _client.StartAsync();

            // Initialize the CommandHandler
            await _commandHandler.InitializeAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}
