using MrPorker.Api.Controllers;
using MrPorker.Services;

namespace MrPorker.Api
{
    public static class Router
    {
        public static void ConfigureEndpoints(IEndpointRouteBuilder app)
        {
            var botApi = app.MapGroup("/bot");

            botApi.MapGet("/ping", async (BotService bot) =>
            {
                var controller = new PingController(bot);
                return await controller.Ping();
            });

            // Add other endpoints here
        }
    }
}
