using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MrPorker.Configs;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MrPorker.Services
{
    public class BotService(DiscordSocketClient client, IServiceProvider services, InteractionService interactionService, BotConfig config)
    {
        public async Task RunAsync()
        {
            client.Ready += OnReadyAsync;
            client.InteractionCreated += OnInteractionCreatedAsync;
            client.MessageReceived += OnMessageReceivedAsync;

            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            await client.LoginAsync(TokenType.Bot, config.BotToken);
            await client.StartAsync();
        }

        #region Handlers

        private async Task OnReadyAsync()
        {
            // If you're using guild-specific commands, use the guild ID instead of null
            //await interactionService.AddCommandsGloballyAsync();
            await interactionService.RegisterCommandsToGuildAsync(config.GuildHideoutId);
            await SendMessageToGeneralAsync("good heavens");
        }

        private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(client, interaction);
            await interactionService.ExecuteCommandAsync(ctx, services);
        }

        private async Task OnMessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return; // Ignore bot's own messages

            if (message.Content.ToLower().Contains("hello"))
                await message.Channel.SendMessageAsync("Hello!");

            var twitterLink = ReplaceTwitterLinks(message.Content);
            if (twitterLink != null)
                await message.Channel.SendMessageAsync(twitterLink);
        }

        private string? ReplaceTwitterLinks(string message)
        {
            var match = Regex.Match(message, config.TwitterLinkRegex);
            return match.Success ? $"{config.TwitterLinkReplacementHost}{match.Groups[3].Value}" : null;
        }

        #endregion

        public async Task SendMessageToGeneralAsync(string content)
        {
            if (await client.GetChannelAsync(config.ChannelGeneralId) is IMessageChannel channel)
                await channel.SendMessageAsync(content);
            else
                Console.WriteLine($"Failed to send message to General, ID: {config.ChannelGeneralId}");
        }

        public async Task SendFileToGeneralAsync(MemoryStream imageStream)
        {
            if (await client.GetChannelAsync(config.ChannelGeneralId) is IMessageChannel channel)
                await channel.SendFileAsync(imageStream, "output.png");
            else
                Console.WriteLine($"Failed to send file to General, ID: {config.ChannelGeneralId}");
        }
    }
}
