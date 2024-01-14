using MrPorker.Configs;
using MrPorker.Data.Dtos;
using MrPorker.Data.Enums;
using Newtonsoft.Json;

namespace MrPorker.Services
{
    public class FirebasePollingService
    {
        private readonly BotConfig _botConfig;
        private readonly MeasurementService _measurementService;
        private readonly HttpClient _httpClient;
        private readonly TimeSpan _pollingInterval;
        private readonly string _firebaseUrl;

        public FirebasePollingService(IHttpClientFactory httpClientFactory, BotConfig config, MeasurementService measurementsService)
        {
            _botConfig = config;
            _measurementService = measurementsService;
            _httpClient = httpClientFactory.CreateClient("FirebasePollingClient");
            _pollingInterval = TimeSpan.FromSeconds(config.FirebasePollingInSeconds);
            _firebaseUrl = config.FirebaseUrl;
        }

        public async Task StartPollingAsync()
        {
            while (true)
            {
                var measurements = await ConsumeDataAsync();
                if (measurements != null)
                {
                    if (measurements.Paul != null)
                    {
                        await _measurementService.AddMeasurementAsync(measurements.Paul, Competitor.Paul);
                    }

                    if (measurements.Addymer != null)
                    {
                        await _measurementService.AddMeasurementAsync(measurements.Addymer, Competitor.Addymer);
                    }

                    if (measurements.Alex != null)
                    {
                        await _measurementService.AddMeasurementAsync(measurements.Alex, Competitor.Alex);
                    }
                }

                await Task.Delay(_pollingInterval);
            }
        }

        // TODO: handle ensure success status code exception
        private async Task<FirebaseDto?> ConsumeDataAsync()
        {
            // Read data
            var response = await _httpClient.GetAsync(_firebaseUrl + ".json?auth=" + _botConfig.FirebaseDatabaseSecret);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<FirebaseDto>(json);

            if (data != null)
            {
                // Delete data
                var deleteResponse = await _httpClient.DeleteAsync(_firebaseUrl + ".json?auth=" + _botConfig.FirebaseDatabaseSecret);
                deleteResponse.EnsureSuccessStatusCode();
            }

            return data;
        }
    }
}
