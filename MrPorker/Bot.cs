using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrPorker
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly CommandHandler _commandHandler;

        public Bot()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _commandHandler = new CommandHandler(_client, _commands);

            // Other initialization code like services, logging, etc.
        }

        public async Task RunAsync()
        {
            await _commandHandler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, "your_bot_token_here");
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}
