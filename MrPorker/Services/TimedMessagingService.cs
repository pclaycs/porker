using Discord;
using MrPorker.Configs;

namespace MrPorker.Services
{
    public class TimedMessagingService
    {
        private readonly BotConfig _botConfig;
        private readonly BotService _botService;
        private readonly DatabaseService _databaseService;
        private readonly TimeSpan _messageTime;
        private bool _messageSentToday = false;

        private PersonalTrainerBotService _personalTrainerBot;
        private HogHoganBotService _hogHoganBot;

        public TimedMessagingService(BotService botService, HogHoganBotService hogHoganBotService, DatabaseService databaseService, BotConfig botConfig)
        {
            _botConfig = botConfig;
            _botService = botService;
            _databaseService = databaseService;
            _messageTime = new TimeSpan(4, 00, 0);

            _personalTrainerBot = new PersonalTrainerBotService(botConfig, databaseService);
            _hogHoganBot = hogHoganBotService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _personalTrainerBot.RunAsync();

            while (!cancellationToken.IsCancellationRequested)
            {
                var now = DateTime.Now.TimeOfDay;

                // Check if the current time has passed the message time and if the message hasn't been sent today
                if (now > _messageTime && !_messageSentToday)
                {
                    _messageSentToday = await ConstructAndSendEmbed();
                }
                else if (now < _messageTime)
                {
                    _messageSentToday = false;
                }


                // Wait for an hour before checking the time again
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }

        public async Task<bool> ConstructAndSendEmbed()
        {
            var day = await _databaseService.GetDaysSinceFirstMeasurementAsync();
            if (day < 1) return false;

            var phrase = await _databaseService.GetPhraseByIdAsync(day);
            if (phrase == null) return false;

            var result = await SendEmbedAsync(day, phrase);

            await _personalTrainerBot.Judge();

            return result;
        }

        public async Task<bool> SendStarterEmbedAsync(int day, string phrase)
        {
            //await _personalTrainerBot.GiveAllRoles();
            return await SendEmbedAsync(day, phrase);
        }

        public async Task<bool> SendEmbedAsync(int day, string phrase)
        {
            var embed = new EmbedBuilder();
            embed.Title = $"DAY {day}";
            embed.Description = phrase;

            embed.ThumbnailUrl = day < 312
                ? "https://i.imgur.com/a4iB71L.png"
                : "https://i.imgur.com/Wrwx5w1.png";

            if (day == 0)
            {
                embed.ThumbnailUrl = "https://i.imgur.com/hgX2vDP.png";
                embed.Title = $"WEIGH-IN DAY";
            }

            embed.Color = new Color(88, 101, 242);
            var builtEmbed = embed.Build();

            await _botService.SendEmbedToChannelAsync(builtEmbed, _botConfig.ChannelHideoutId);

            var message = await _botService.SendEmbedToChannelAsync(builtEmbed, _botConfig.ChannelMatchHistoryId);
            if (message == null) return false;

            var message2 = await _botService.SendEmbedToChannelAsync(builtEmbed, _botConfig.ChannelPorkOffGeneralId);
            return message2 != null;
        }
    }

}
