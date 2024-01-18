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

            botApi.MapGet("/ping", async (BotService botService, BotConfig botConfig) =>
            {
                var controller = new PingController(botService, botConfig);
                return await controller.Ping();
            });

            //botApi.MapPost("/measurement", async (MeasurementDto measurementDto, MeasurementService measurementService) =>
            //{
            //    //await measurementService.AddMeasurementAsync(measurementDto, Data.Enums.Competitor.Addymer);
            //    //await measurementService.AddMeasurementAsync(measurementDto, Data.Enums.Competitor.Alex);
            //    await measurementService.AddMeasurementAsync(measurementDto, Data.Enums.Competitor.Paul);
            //});

            botApi.MapGet("/start", async (TimedMessagingService timedMessagingService) =>
            {
                await timedMessagingService.SendStarterEmbedAsync(0, "Hello you fat fucks! It's time to weigh-in!");
            });

            botApi.MapGet("/unlock", async (FirebaseService firebaseService) =>
            {
                await firebaseService.UnlockPhone();
            });

            botApi.MapGet("/keep", async (FirebaseService firebaseService) =>
            {
                await firebaseService.KeepPhotos();
            });

            botApi.MapGet("/retake", async (FirebaseService firebaseService) =>
            {
                await firebaseService.RetakePhotos();
            });
        }
    }
}
