//using Discord.Commands;
//using Discord.WebSocket;
//using Discord;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MrPorker.Configs;
//using Discord.Interactions;
//using System.Reflection;

//namespace MrPorker
//{
//    public class Bot
//    {
//        private readonly DiscordSocketClient _client;
//        private readonly InteractionService _interactionService;
//        private readonly string _token;

//        public Bot(BotConfig configuration, InteractionService interactionService)
//        {
//            _client = new DiscordSocketClient(new DiscordSocketConfig
//            {
//                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
//            });

//            _interactionService = interactionService;

//            //_commands = new CommandService();
//            //_commandHandler = new CommandHandler(_client, _commands);

//            _token = configuration.BotToken;

//            // Other initialization code like services, logging, etc.
//        }

//        public async Task RunAsync()
//        {
//            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);

//            _client.Ready += ClientReadyAsync;

//            await _client.LoginAsync(TokenType.Bot, _token);
//            await _client.StartAsync();

//            // Block this task until the program is closed.
//            await Task.Delay(-1);
//        }

//        private async Task _client_Ready()
//        {
//            ulong guildId = 388416523049500682;

//            await _interactionService.RegisterCommandsToGuildAsync(guildId);

//            ulong channelId = 1035886137996234832;

//            var channel = _client.GetChannel(channelId) as IMessageChannel;
//            if (channel != null)
//            {
//                await channel.SendMessageAsync("Hello fuckers im new and improved");
//            }
//            else
//            {
//                Console.WriteLine("Channel not found.");
//            }
//        }
//    }
//}


using Discord;
using Discord.WebSocket;
using MrPorker.Configs;
using MrPorker.Services;
using System.Threading.Tasks;

namespace MrPorker
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly BotConfig _config;

        public Bot(DiscordSocketClient client, CommandHandler commandHandler, BotConfig config)
        {
            _client = client;
            _config = config;
            _commandHandler = commandHandler;
        }

        public async Task RunAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _config.BotToken);
            await _client.StartAsync();

            // Initialize the CommandHandler
            await _commandHandler.InitializeAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}
