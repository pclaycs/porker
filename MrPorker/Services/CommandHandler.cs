using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MrPorker.Configs;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MrPorker.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _services;
        private readonly BotConfig _botConfig;

        public CommandHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services, BotConfig botConfig)
        {
            _client = client;
            _interactionService = interactionService;
            _services = services;
            _botConfig = botConfig;
        }

        public async Task InitializeAsync()
        {
            _client.Ready += OnReadyAsync;
            _client.InteractionCreated += OnInteractionCreatedAsync;
            _client.MessageReceived += OnMessageReceivedAsync;

            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task OnReadyAsync()
        {
            // If you're using guild-specific commands, use the guild ID instead of null
            await _interactionService.RegisterCommandsToGuildAsync(_botConfig.GuildHideoutId);
            if (_client.GetChannel(_botConfig.ChannelGeneralId) is IMessageChannel channel)
            {
                await channel.SendMessageAsync("Hello fuckers im new and improved");
            }
            else
            {
                Console.WriteLine("Channel not found.");
            }
        }

        private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _services);
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
            var match = Regex.Match(message, _botConfig.TwitterLinkRegex);
            return match.Success ? $"{_botConfig.TwitterLinkReplacementHost}{match.Groups[3].Value}" : null;
        }
    }
}
