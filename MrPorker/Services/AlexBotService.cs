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
            await SendMessageToGeneralAsync("id say im all ears, but im ALL BONES");
            await SendMessageToGeneralAsync("AHAHAHAHAHAHAHAHHAHAAHAHAHAHAHA");
            await SendMessageToGeneralAsync("AHAHAHAHAHAHAHAHAHAHAHAHA");
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
