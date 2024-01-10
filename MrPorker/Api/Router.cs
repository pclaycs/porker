using MrPorker.Api.Controllers;
using MrPorker.Api.Controllers.Measurement;
using MrPorker.Configs;
using MrPorker.Data.Dtos;
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

            botApi.MapPost("/measurement", async (MeasurementDto measurementDto, BotConfig botConfig, BotService bot, DatabaseService databaseService) =>
            {
                var controller = new MeasurementController(botConfig, bot, databaseService);
                return await controller.AddMeasurementAsync(measurementDto);
            });

            // Add other endpoints here
        }
    }
}
