using MrPorker.Services;

namespace MrPorker.Api.Controllers
{
    public class PingController(BotService botService)
    {
        private readonly BotService _botService = botService;

        public async Task<IResult> Ping()
        {
            await _botService.SendMessageToGeneralAsync("pong");
            return Results.Ok();
        }
    }
}
