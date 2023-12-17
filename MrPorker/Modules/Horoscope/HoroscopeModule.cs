using Discord;
using Discord.Interactions;
using MrPorker.Configs;
using MrPorker.Services;

namespace MrPorker.Commands.Horoscope
{
    public class HoroscopeModule : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly Dictionary<ulong, int> _lastCheckedSigns = new();
        private readonly HoroscopeService _horoscopeService;
        private readonly BotConfig _config;

        public HoroscopeModule(HoroscopeService horoscopeService, BotConfig config) : base()
        {
            _horoscopeService = horoscopeService;
            _config = config;
        }

        [SlashCommand("horoscope2", "Get the horoscope for a specific sign")]
        public async Task HoroscopeAsync(
            [Summary("sign", "Your Zodiac sign")]
            [Choice("Aries", 1),
             Choice("Taurus", 2),
             Choice("Gemini", 3),
             Choice("Cancer", 4),
             Choice("Leo", 5),
             Choice("Virgo", 6),
             Choice("Libra", 7),
             Choice("Scorpio", 8),
             Choice("Sagittarius", 9),
             Choice("Capricorn", 10),
             Choice("Aquarius", 11),
             Choice("Pisces", 12)] int sign = 0)
        {

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"mr porker predicts...")
                .WithThumbnailUrl(_config.HoroscopeThumbnail);

            ulong userId = Context.User.Id;

            if (sign < 1 || sign > 12)
            {
                if (_lastCheckedSigns.ContainsKey(userId))
                {
                    sign = _lastCheckedSigns[userId];
                }
                else
                {
                    embedBuilder
                        .WithDescription("idk ur star sign bro")
                        .WithFooter(footer => footer.Text = "type it in and i'll remember ig")
                        .WithColor(Color.DarkRed);

                    await RespondAsync(embed: embedBuilder.Build());
                    return;
                }
            }

            _lastCheckedSigns[userId] = sign;

            // Implement horoscope retrieval logic here
            var horoscope = await _horoscopeService.GetHoroscopeAsync(sign);

            if (horoscope != null)
            {
                embedBuilder
                    .WithDescription(horoscope.Horoscope)
                    .WithFooter(footer => footer.Text = $"{horoscope.Sign}  •  {horoscope.Date}")
                    .WithColor(Color.DarkPurple);
            }
            else
            {
                embedBuilder
                    .WithDescription("how the fuck should i know")
                    .WithFooter(footer => footer.Text = "u think im psychic or sum shit?")
                    .WithColor(Color.DarkRed);
            }

            // Build the embed
            var embed = embedBuilder.Build();
            await RespondAsync(embed: embed);
        }
    }
}