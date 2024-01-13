using Discord.Interactions;
using Discord;
using System.Reflection;
using Discord.WebSocket;
using MrPorker.Configs;

namespace MrPorker.Services
{
    public class AddymerBotService
    {
        private readonly DiscordSocketClient _client;
        private readonly BotConfig _config;

        public AddymerBotService(BotConfig config)
        {
            _config = config;
            _client = new DiscordSocketClient();
        }

        public async Task RunAsync()
        {
            _client.Ready += OnReadyAsync;
            await _client.LoginAsync(TokenType.Bot, _config.AddymerBotToken);
            await _client.StartAsync();
        }

        private async Task OnReadyAsync()
        {
            await SendMessageToGeneralAsync("豚ブレードを解き放つ準備が整った");
        }

        public async Task SendMessageToGeneralAsync(string content)
        {
            if (await _client.GetChannelAsync(_config.ChannelGeneralId) is IMessageChannel channel)
                await channel.SendMessageAsync(content);
            else
                Console.WriteLine($"Failed to send message to General, ID: {_config.ChannelGeneralId}");
        }

        public async Task SendFileToGeneralAsync(MemoryStream imageStream)
        {
            if (await _client.GetChannelAsync(_config.ChannelGeneralId) is IMessageChannel channel)
                await channel.SendFileAsync(imageStream, "output.png");
            else
                Console.WriteLine($"Failed to send file to General, ID: {_config.ChannelGeneralId}");
        }
    }
}
