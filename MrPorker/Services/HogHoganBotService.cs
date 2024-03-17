using Discord.WebSocket;
using Discord;
using MrPorker.Configs;
using MrPorker.Data.Enums;
using MrPorker.Data.Dtos;

namespace MrPorker.Services
{
    public class HogHoganBotService
    {
        private readonly DatabaseService _databaseService;
        private readonly DiscordSocketClient _client;
        private readonly BotConfig _config;

        public HogHoganBotService(BotConfig config, DatabaseService databaseService)
        {
            _databaseService = databaseService;

            _config = config;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions,
                AlwaysDownloadUsers = true // Enable user caching
            });
        }

        public async Task RunAsync()
        {
            _client.Ready += OnReadyAsync;

            await _client.LoginAsync(TokenType.Bot, _config.HogHoganBotToken);
            await _client.StartAsync();
        }

        private async Task OnReadyAsync()
        {
            await SendMessageToChannelAsync("shut the fuck up", _config.ChannelGeneralId);
        }

        public async Task SendMessageToChannelAsync(string content, ulong channelId)
        {
            if (await _client.GetChannelAsync(channelId) is IMessageChannel channel)
                await channel.SendMessageAsync(content);
            else
                Console.WriteLine($"Failed to send message to channel ID: {channelId}");
        }

        public async Task<bool> SendStrengthRanking()
        {
            var people = await GetPeople();
            var scores = CharacterRanking.PopulateScoreHolder(people);

            if (scores == null) return false;
            return await SendEmbedForStat("STRENGTH", scores.Strength);
        }

        public async Task<bool> SendEnduranceRanking()
        {
            var people = await GetPeople();
            var scores = CharacterRanking.PopulateScoreHolder(people);

            if (scores == null) return false;
            return await SendEmbedForStat("ENDURANCE", scores.Endurance);
        }


        public async Task<bool> SendAgilityRanking()
        {
            var people = await GetPeople();
            var scores = CharacterRanking.PopulateScoreHolder(people);

            if (scores == null) return false;
            return await SendEmbedForStat("AGILITY", scores.Agility);
        }


        public async Task<bool> SendOverallRanking()
        {
            var people = await GetPeople();
            var scores = CharacterRanking.PopulateScoreHolder(people);

            if (scores == null) return false;
            return await SendEmbedForStat("THE PORK-OFF", scores.Overall);
        }


        private async Task<Dictionary<SocketGuildUser, MeasurementDto>> GetPeople()
        {
            var people = new Dictionary<SocketGuildUser, MeasurementDto>();

            var paulLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Paul);
            var addymerLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Addymer);
            var alexLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Alex);
            var eunoraLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Eunora);
            var bluLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Blu);

            var paulUser = GetUserByCompetitor(Competitor.Paul);
            var addymerUser = GetUserByCompetitor(Competitor.Addymer);
            var alexUser = GetUserByCompetitor(Competitor.Alex);
            var eunoraUser = GetUserByCompetitor(Competitor.Eunora);
            var bluUser = GetUserByCompetitor(Competitor.Blu);

            
            if (paulLatest != null && paulUser != null)
            {
                paulLatest.Height = 177;
                people.Add(paulUser, paulLatest);
            }

            if (addymerLatest != null && addymerUser != null)
            {
                addymerLatest.Height = 175;
                people.Add(addymerUser, addymerLatest);
            }

            if (alexLatest != null && alexUser != null)
            {
                alexLatest.Height = 192;
                people.Add(alexUser, alexLatest);
            }

            if (eunoraLatest != null && eunoraUser != null)
            {
                eunoraLatest.Height = 190;
                people.Add(eunoraUser, eunoraLatest);
            }

            if (bluLatest != null && bluUser != null)
            {
                bluLatest.Height = 175;
                people.Add(bluUser, bluLatest);
            }

            return people;
        }

        private SocketGuildUser? GetUserByCompetitor(Competitor competitor)
        {
            var guild = _client.GetGuild(_config.GuildHideoutId);
            SocketGuildUser? user = null;

            if (competitor == Competitor.Paul)
                user = guild?.GetUser(_config.PaulDiscordId);

            if (competitor == Competitor.Addymer)
                user = guild?.GetUser(_config.AddymerDiscordId);

            if (competitor == Competitor.Eunora)
                user = guild?.GetUser(_config.EunoraDiscordId);

            if (competitor == Competitor.Alex)
                user = guild?.GetUser(_config.AlexDiscordId);

            if (competitor == Competitor.Blu)
                user = guild?.GetUser(_config.BluDiscordId);

            return user;
        }

        public async Task SendCompetitorUpdate(MeasurementDto measurement, Competitor competitor, MeasurementDto? lastKnownMeasurement)
        {
            if (measurement == null || lastKnownMeasurement == null) return;

            SocketGuildUser? user = GetUserByCompetitor(competitor);
            if (user == null) return;

            var embed = new EmbedBuilder();
            embed.Title = $"{user.DisplayName.ToUpperInvariant()} WEIGHED-IN";

            var nameField = new EmbedFieldBuilder();
            var eloField = new EmbedFieldBuilder();
            var deltaField = new EmbedFieldBuilder();

            //rankField.IsInline = true;
            nameField.IsInline = true;
            eloField.IsInline = true;
            deltaField.IsInline = true;

            //rankField.Name = "#";
            nameField.Name = "Stat";
            eloField.Name = "ELO";
            deltaField.Name = "-/+";

            nameField.Value = "`str`\n`end`\n`agl`\n`pork`";
            eloField.Value = $"`{measurement.Strength}`\n`{measurement.Endurance}`\n`{measurement.Agility}`\n`{measurement.Overall}`";

            var strengthDelta = $"{(measurement.Strength - lastKnownMeasurement.Strength < 0 ? "" : "+")}{measurement.Strength - lastKnownMeasurement.Strength:F0}";
            var enduranceDelta = $"{(measurement.Endurance - lastKnownMeasurement.Endurance < 0 ? "" : "+")}{measurement.Endurance - lastKnownMeasurement.Endurance:F0}";
            var agilityDelta = $"{(measurement.Agility - lastKnownMeasurement.Agility < 0 ? "" : "+")}{measurement.Agility - lastKnownMeasurement.Agility:F0}";
            var overallDelta = $"{(measurement.Overall - lastKnownMeasurement.Overall < 0 ? "" : "+")}{measurement.Overall - lastKnownMeasurement.Overall:F0}";

            deltaField.Value = $"`{strengthDelta}`\n`{enduranceDelta}`\n`{agilityDelta}`\n`{overallDelta}`";

            embed.AddField(nameField);
            embed.AddField(eloField);
            embed.AddField(deltaField);

            embed.ThumbnailUrl = user.GetDisplayAvatarUrl();
            embed.ThumbnailUrl.Replace(".gif", ".png");

            embed.Color = new Color(129, 131, 132);
            if (measurement.Overall - lastKnownMeasurement.Overall > 0)
                embed.Color = new Color(31, 139, 76);
            else if (measurement.Overall - lastKnownMeasurement.Overall < 0)
                embed.Color = new Color(237, 66, 69);

            var builtOverallEmbed = embed.Build();
            await SendEmbedToChannelAsync(builtOverallEmbed, _config.ChannelPorkCentralId);
        }

        public async Task<bool> SendEmbedForStat(string statName, IEnumerable<KeyValuePair<SocketGuildUser, double>> scores)
        {
            var embed = new EmbedBuilder();

            embed.Title = statName;

            //var rankField = new EmbedFieldBuilder();
            var nameField = new EmbedFieldBuilder();
            var eloField = new EmbedFieldBuilder();
            var gradeField = new EmbedFieldBuilder();

            //rankField.IsInline = true;
            nameField.IsInline = true;
            eloField.IsInline = true;
            gradeField.IsInline = true;

            //rankField.Name = "#";
            nameField.Name = "Porker";
            eloField.Name = "ELO";
            gradeField.Name = "★";

            //rankField.Value = string.Join("\n", scores.Select((kvp, index) => $"{index + 1}"));
            nameField.Value = string.Join("\n", scores.Select((kvp, index) => $"`{kvp.Key.DisplayName}`")); // \t\t{(int)Math.Round(kvp.Value)}\t{CharacterRanking.GetGrade((int)Math.Round(kvp.Value))}"));
            eloField.Value = string.Join("\n", scores.Select((kvp, index) => $"`{(int)Math.Round(kvp.Value)}`")); // \t{CharacterRanking.GetGrade((int)Math.Round(kvp.Value))}"));
            gradeField.Value = string.Join("\n", scores.Select((kvp, index) => $"`{CharacterRanking.GetGrade((int)Math.Round(kvp.Value))}`"));

            //embed.AddField(rankField);
            embed.AddField(nameField);
            embed.AddField(eloField);
            embed.AddField(gradeField);

            if (statName == "STRENGTH")
            {
                embed.Color = new Color(255, 105, 97);
                embed.ThumbnailUrl = "https://i.imgur.com/NIwsSCt.png";
            }

            if (statName == "ENDURANCE")
            {
                embed.Color = new Color(240, 189, 71);
                embed.ThumbnailUrl = "https://i.imgur.com/6kcTXLp.png";
            }

            if (statName == "AGILITY")
            {
                embed.Color = new Color(119, 221, 119);
                embed.ThumbnailUrl = "https://i.imgur.com/XHWEvk3.png";
            }

            if (statName == "THE PORK-OFF")
            {
                //embed.Color = new Color(154, 183, 217);
                embed.Color = new Color(240, 210, 170);
                embed.ThumbnailUrl = scores.Select((kvp, index) => kvp.Key.GetDisplayAvatarUrl().Replace(".gif", ".png")).FirstOrDefault() ?? "https://i.imgur.com/hgX2vDP.png";
                embed.ImageUrl = "https://i.imgur.com/B6qpFvU.png";
            }

            var builtEmbed = embed.Build();
            var message = await SendEmbedToChannelAsync(builtEmbed, _config.ChannelPorkCentralId);

            return message != null;
        }

        public async Task<IUserMessage?> SendEmbedToChannelAsync(Embed embed, ulong channelId, MessageComponent? components = null)
        {
            if (await _client.GetChannelAsync(channelId) is IMessageChannel channel)
                return await channel.SendMessageAsync(embed: embed, components: components);
            else
                Console.WriteLine($"Failed to send file to channel ID: {channelId}");

            return null;
        }
    }
}
