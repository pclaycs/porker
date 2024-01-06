using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MrPorker.Configs;
using NodaTime;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MrPorker.Services
{
    public class BotService(DiscordSocketClient client, IServiceProvider services, InteractionService interactionService, BotConfig config)
    {
        public async Task RunAsync()
        {

            client.Ready += OnReadyAsync;
            client.InteractionCreated += OnInteractionCreatedAsync;
            client.MessageReceived += OnMessageReceivedAsync;
            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            await client.LoginAsync(TokenType.Bot, config.BotToken);
            await client.StartAsync();
        }

        #region Handlers

        private async Task OnReadyAsync()
        {
            // If you're using guild-specific commands, use the guild ID instead of null
            await interactionService.RegisterCommandsToGuildAsync(config.GuildHideoutId);
            await SendMessageToGeneralAsync("good heavens");
        }

        private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(client, interaction);
            await interactionService.ExecuteCommandAsync(ctx, services);
        }

        private async Task OnMessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return; // Ignore bot's own messages

            var twitterLink = ReplaceTwitterLinks(message.Content);
            if (twitterLink != null)
                await message.Channel.SendMessageAsync(twitterLink);

            var extractedTime = ExtractTimeFromString(message.Content);
            if (extractedTime != null)
                await message.Channel.SendMessageAsync(ConvertAndDisplayTimeForUsers(extractedTime ?? new DateTime()));
            
        }

        #region Twitter Embeds
        private string? ReplaceTwitterLinks(string message)
        {
            var match = Regex.Match(message, config.TwitterLinkRegex);
            return match.Success ? $"{config.TwitterLinkReplacementHost}{match.Groups[3].Value}" : null;
        }
        #endregion


        #region Timezone Conversions
        private static DateTime? ExtractTimeFromString(string input)
        {
            var matches = Regex.Matches(input, @"\d{1,2}(:\d{2})?\s?(am|pm|AM|PM)?");
            foreach (Match match in matches)
            {
                string time = match.Value.Replace(" ", ""); // Remove any spaces
                if (DateTime.TryParseExact(time, new[] { "htt", "h:mmtt", "HHmm", "H:mm", "hmm", "h:mm", "HH:mm", "H:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }
            }

            return null;
        }

        private static string ConvertAndDisplayTimeForUsers(DateTime extractedTime)
        {
            var taetiesTime = "America/Chicago";
            var humanTime = "Australia/Melbourne";

            LocalDateTime localDateTime = LocalDateTime.FromDateTime(extractedTime);
            DateTimeZone opsTimeZone = DateTimeZoneProviders.Tzdb[taetiesTime];
            ZonedDateTime opsZonedDateTime = localDateTime.InZoneLeniently(opsTimeZone);


            DateTimeZone userTimeZone = DateTimeZoneProviders.Tzdb[humanTime];
            ZonedDateTime userZonedDateTime = opsZonedDateTime.WithZone(userTimeZone);

            return $"{extractedTime} in taeties time is {userZonedDateTime.ToDateTimeUnspecified()} in human time.";
        }
        #endregion




        #endregion

        public async Task SendMessageToGeneralAsync(string content)
        {
            if (await client.GetChannelAsync(config.ChannelGeneralId) is IMessageChannel channel)
                await channel.SendMessageAsync(content);
            else
                Console.WriteLine($"Failed to send message to General, ID: {config.ChannelGeneralId}");
        }
    }
}
