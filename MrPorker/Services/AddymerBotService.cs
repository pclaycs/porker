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
            //await SendMessageToChannelAsync("every hog.. has his day...", _config.ChannelGeneralId);
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

        public async Task<IUserMessage?> SendImageEmbedToChannelAsync(MemoryStream imageStream, ulong threadId)
        {
            var thread = _client.GetGuild(_config.GuildPorkOffId).GetThreadChannel(threadId);
            if (thread == null) return null;

            var embed = new EmbedBuilder()
                .WithImageUrl("attachment://output.png")
                .WithColor(Color.Blue)
            .Build();

            return await thread.SendFileAsync(imageStream, "output.png", embed: embed);
        }

        public async Task<IUserMessage?> SendFileToThreadAsync(MemoryStream imageStream, ulong threadId)
        {
            var thread = _client.GetGuild(_config.GuildPorkOffId).GetThreadChannel(threadId);
            if (thread == null) return null;

            return await thread.SendFileAsync(imageStream, "output.png");
        }
    }
}
