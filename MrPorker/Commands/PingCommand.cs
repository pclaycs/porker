using Discord.Interactions;

namespace MrPorker.Commands
{
    public class PingCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Replies with Pong!")]
        public async Task PingAsync()
        {
            await RespondAsync("Pong!");
        }
    }
}