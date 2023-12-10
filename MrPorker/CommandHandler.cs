using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrPorker
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook command execution events here
            _client.SlashCommandExecuted += SlashCommandHandler;

            // Register commands here
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            // Handle commands
        }
    }

}
