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

        private readonly TimeSpan _strengthTime;
        private bool _strengthSentToday = false;
        private readonly TimeSpan _enduranceTime;
        private bool _enduranceSentToday = false;
        private readonly TimeSpan _agilityTime;
        private bool _agilitySentToday = false;
        private readonly TimeSpan _overallTime;
        private bool _overallSentToday = false;

        private PersonalTrainerBotService _personalTrainerBot;
        private HogHoganBotService _hogHoganBot;

        public TimedMessagingService(BotService botService, HogHoganBotService hogHoganBotService, DatabaseService databaseService, BotConfig botConfig)
        {
            _botConfig = botConfig;
            _botService = botService;
            _databaseService = databaseService;
            _messageTime = new TimeSpan(4, 00, 0);

            _strengthTime = new TimeSpan(16, 00, 0);
            _enduranceTime = new TimeSpan(16, 00, 0);
            _agilityTime = new TimeSpan(16, 00, 0);
            _overallTime = new TimeSpan(16, 00, 0);

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

                if (now > _strengthTime && !_strengthSentToday)
                {
                    _strengthSentToday = await _hogHoganBot.SendStrengthRanking();
                }
                else if (now < _strengthTime)
                {
                    _strengthSentToday = false;
                }

                if (now > _enduranceTime && !_enduranceSentToday)
                {
                    _enduranceSentToday = await _hogHoganBot.SendEnduranceRanking();
                }
                else if (now < _enduranceTime)
                {
                    _enduranceSentToday = false;
                }

                if (now > _agilityTime && !_agilitySentToday)
                {
                    _agilitySentToday = await _hogHoganBot.SendAgilityRanking();
                }
                else if (now < _agilityTime)
                {
                    _agilitySentToday = false;
                }

                if (now > _overallTime && !_overallSentToday)
                {
                    _overallSentToday = await _hogHoganBot.SendOverallRanking();
                }
                else if (now < _overallTime)
                {
                    _overallSentToday = false;
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
            await _personalTrainerBot.GiveAllRoles();
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

            var message = await _botService.SendEmbedToChannelAsync(builtEmbed, _botConfig.ChannelPorkboardId);
            if (message == null) return false;

            var component = new ComponentBuilder()
                .WithButton(label: "oink!",
                            style: ButtonStyle.Link,
                            url: $"https://discord.com/channels/{_botConfig.GuildHideoutId}/{_botConfig.ChannelPorkboardId}/{message.Id}",
                            emote: new Emoji("🐷"))
                .Build();

            await _botService.SendEmbedToChannelAsync(builtEmbed, _botConfig.ChannelGeneralId, component);
            await _botService.SendEmbedToChannelAsync(builtEmbed, _botConfig.ChannelPorkCentralId, component);

            return true;
        }
    }

}
