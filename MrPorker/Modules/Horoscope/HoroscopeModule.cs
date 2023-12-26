using Discord;
using Discord.Interactions;
using MrPorker.Configs;
using MrPorker.Services;

namespace MrPorker.Commands.Horoscope
{
    public class HoroscopeModule(DatabaseService databaseService, HoroscopeService horoscopeService, BotConfig config) : InteractionModuleBase<SocketInteractionContext>()
    {
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
            await DeferAsync();

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"mr porker predicts...")
                .WithThumbnailUrl(config.HoroscopeThumbnail);

            ulong userId = Context.User.Id;
            if (sign < 1 || sign > 12)
            {
                var storedSign = await databaseService.GetHoroscopeSignAsync(userId);
                if (storedSign != null)
                {
                    sign = storedSign.Sign;
                }
                else
                {
                    embedBuilder
                        .WithDescription("idk ur star sign bro")
                        .WithFooter(footer => footer.Text = "do it once and i'll remember")
                        .WithColor(Color.DarkRed);

                    await FollowupAsync(embed: embedBuilder.Build());
                    return;
                }
            }
            else
            {
                await databaseService.SetHoroscopeSignAsync(userId, sign);
            }

            var horoscope = await horoscopeService.GetHoroscopeAsync(sign);
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

            var embed = embedBuilder.Build();
            await FollowupAsync(embed: embed);
        }
    }
}