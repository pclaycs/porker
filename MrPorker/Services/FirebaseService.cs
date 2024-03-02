using MrPorker.Configs;
using MrPorker.Data.Dtos;
using MrPorker.Data.Enums;
using Newtonsoft.Json;

using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace MrPorker.Services
{
    public class FirebaseService
    {
        private readonly BotConfig _botConfig;
        private readonly MeasurementService _measurementService;
        private readonly HttpClient _httpClient;
        private readonly TimeSpan _pollingInterval;
        private readonly string _firebaseUrl;
        private readonly string _firebaseMessagingToken;

        public FirebaseService(IHttpClientFactory httpClientFactory, BotConfig config, MeasurementService measurementsService)
        {
            _botConfig = config;
            _measurementService = measurementsService;
            _httpClient = httpClientFactory.CreateClient("FirebasePollingClient");
            _pollingInterval = TimeSpan.FromSeconds(config.FirebasePollingInSeconds);
            _firebaseUrl = config.FirebaseUrl;
            _firebaseMessagingToken = config.FirebaseMessagingToken;

            // Initialize the Firebase app
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(config.FirebaseServiceAccountKeyJsonFilePath)
            });
        }

        public async Task StartPollingAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }

                await Task.Delay(_pollingInterval);
            }
        }

        private async Task SendFirebaseMessage(string key, string value)
        {
            // Create a message payload
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { key, value }
                },
                Token = _firebaseMessagingToken,
            };

            // Send a message to the device corresponding to the provided registration token
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Successfully sent message: " + response);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending message: " + e.Message);
            }
        }

        public async Task UnlockPhone()
        {
            await SendFirebaseMessage("unlock", "true");
        }

        public async Task KeepPhotos()
        {
            await SendFirebaseMessage("action", "save");
        }

        public async Task RetakePhotos()
        {
            await SendFirebaseMessage("action", "retake");
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
