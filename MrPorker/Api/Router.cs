using MrPorker.Api.Controllers;
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

            botApi.MapPost("/measurement", async (MeasurementDto measurementDto, MeasurementService measurementService) =>
            {
                await measurementService.AddMeasurementAsync(measurementDto, Data.Enums.Competitor.Addymer);
                await measurementService.AddMeasurementAsync(measurementDto, Data.Enums.Competitor.Alex);
                await measurementService.AddMeasurementAsync(measurementDto, Data.Enums.Competitor.Paul);
            });

            // Add other endpoints here
        }
    }
}
