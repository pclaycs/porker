using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using MrPorker.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    }
}
