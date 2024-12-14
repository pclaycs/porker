using MrPorker.Configs;
using MrPorker.Services;

namespace MrPorker.Api.Controllers
{
    public class PingController(BotService botService, BotConfig botConfig)
    {
        private readonly BotService _botService = botService;

        public async Task<IResult> Ping()
        {
            await _botService.SendMessageToChannelAsync("pong", botConfig.ChannelPorkOffGeneralId);
            return Results.Ok();
        }
    }
}
