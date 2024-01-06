using Discord.Interactions;

namespace MrPorker.Commands
{
    public class PingModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "pong it up lolz")]
        public async Task PingAsync()
        {
            await RespondAsync("Pong!");
        }
    }
}