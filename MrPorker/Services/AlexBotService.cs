using Discord.Interactions;
using Discord;
using System.Reflection;
using Discord.WebSocket;
using MrPorker.Configs;

namespace MrPorker.Services
{
    public class AlexBotService
    {
        private readonly DiscordSocketClient _client;
        private readonly BotConfig _config;

        public AlexBotService(BotConfig config)
        {
            _config = config;
            _client = new DiscordSocketClient();
        }

        public async Task RunAsync()
        {
            _client.Ready += OnReadyAsync;
            await _client.LoginAsync(TokenType.Bot, _config.AlexBotToken);
            await _client.StartAsync();
        }

        private async Task OnReadyAsync()
        {
            // await SendMessageToChannelAsync("GRAARAHGHGHHRAAAAAAH", _config.ChannelGeneralId);
            // await SendMessageToChannelAsync("AHAHAHGRAGGGGAAHAAHA", _config.ChannelGeneralId);
            // await SendMessageToChannelAsync("GRAARG IT FUCKING BURNS", _config.ChannelGeneralId);
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
