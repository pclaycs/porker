using Discord;
using Discord.WebSocket;
using MrPorker.Configs;

namespace MrPorker.Services
{
    public class BluBotService
    {
        private readonly DiscordSocketClient _client;
        private readonly BotConfig _config;

        public BluBotService(BotConfig config)
        {
            _config = config;
            _client = new DiscordSocketClient();
        }

        public async Task RunAsync()
        {
            _client.Ready += OnReadyAsync;
            await _client.LoginAsync(TokenType.Bot, _config.BluBotToken);
            await _client.StartAsync();
        }

        private async Task OnReadyAsync()
        {
            // await SendMessageToChannelAsync("look at the cleanse look at the hooves", _config.ChannelGeneralId);
        }

        public async Task SendMessageToChannelAsync(string content, ulong channelId)
        {
            if (await _client.GetChannelAsync(channelId) is IMessageChannel channel)
                await channel.SendMessageAsync(content);
            else
                Console.WriteLine($"Failed to send message to channel ID: {channelId}");
        }

        public async Task SendFileToChannelAsync(MemoryStream imageStream, ulong channelId)
        {
            if (await _client.GetChannelAsync(channelId) is IMessageChannel channel)
                await channel.SendFileAsync(imageStream, "output.png");
            else
                Console.WriteLine($"Failed to send file to channel ID: {channelId}");
        }
    }
}
