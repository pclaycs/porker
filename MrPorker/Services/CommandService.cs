using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MrPorker.Configs;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MrPorker.Services
{
    public class CommandService(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services, BotConfig botConfig)
    {
        private readonly IServiceProvider _services = services;

        public async Task InitializeAsync()
        {
            client.Ready += OnReadyAsync;
            client.InteractionCreated += OnInteractionCreatedAsync;
            client.MessageReceived += OnMessageReceivedAsync;

            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task OnReadyAsync()
        {
            // If you're using guild-specific commands, use the guild ID instead of null
            await interactionService.RegisterCommandsToGuildAsync(botConfig.GuildHideoutId);
            if (await client.GetChannelAsync(botConfig.ChannelGeneralId) is IMessageChannel channel)
            {
                await channel.SendMessageAsync("good heavens");
            }
            else
            {
                Console.WriteLine("Channel not found.");
            }
        }

        private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(client, interaction);
            await interactionService.ExecuteCommandAsync(ctx, _services);
        }

        private async Task OnMessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return; // Ignore bot's own messages

            var twitterLink = ReplaceTwitterLinks(message.Content);
            if (twitterLink != null)
                await message.Channel.SendMessageAsync(twitterLink);
        }
        private string? ReplaceTwitterLinks(string message)
        {
            var match = Regex.Match(message, botConfig.TwitterLinkRegex);
            return match.Success ? $"{botConfig.TwitterLinkReplacementHost}{match.Groups[3].Value}" : null;
        }
    }
}
