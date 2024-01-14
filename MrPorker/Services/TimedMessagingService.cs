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

        public TimedMessagingService(BotService botService, DatabaseService databaseService, BotConfig botConfig)
        {
            _botConfig = botConfig;
            _botService = botService;
            _databaseService = databaseService;
            _messageTime = new TimeSpan(4, 00, 0); // Example: 8:00 AM
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
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

            return await SendEmbedAsync(day, phrase);
        }

        public async Task<bool> SendEmbedAsync(int day, string phrase)
        {
            var embed = new EmbedBuilder();
            embed.Title = $"DAY {day}";
            embed.Description = phrase;

            embed.ThumbnailUrl = day < 312
                ? "https://media.discordapp.net/attachments/756399558434619452/1163528687136821429/mrporkerproduction.png"
                : "https://media.discordapp.net/attachments/721319129256296448/1163561057420312586/DALLE_2023-10-17_06.34.48_-_Dark_photo_focusing_on_a_big_pigs_face_where_the_boundary_between_beast_and_machine_blurs._Parts_of_its_skin_unveil_the_mechanical_structure_beneath.png";

            if (day == 0)
            {
                embed.ThumbnailUrl = "https://media.discordapp.net/attachments/1035886137996234832/1195959268474626108/mrporker.png";
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
