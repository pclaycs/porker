using Discord;
using Discord.WebSocket;
using MrPorker.Configs;

namespace MrPorker.Services
{
    public class BotService(DiscordSocketClient client, CommandService commandHandler, BotConfig config)
    {
        public async Task RunAsync()
        {
            await client.LoginAsync(TokenType.Bot, config.BotToken);
            await client.StartAsync();

            // Initialize the CommandHandler
            await commandHandler.InitializeAsync();
        }

        public async Task SendMessageAsync(string content)
        {
            if (await client.GetChannelAsync(config.ChannelGeneralId) is IMessageChannel channel)
            {
                await channel.SendMessageAsync(content);
            }
            else
            {
                Console.WriteLine("Channel not found.");
            }
        }
    }
}
