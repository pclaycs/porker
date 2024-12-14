using Discord.WebSocket;
using Discord;
using MrPorker.Configs;
using MrPorker.Data.Enums;
using MrPorker.Data.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.Intrinsics.X86;
using System;

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
            //await SendMessageToChannelAsync("shut the fuck up", _config.ChannelGeneralId);
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

            await UpdateRoles(scores.Overall);

            return await SendEmbedForStat("THE PORK-OFF", scores.Overall);
        }

        public async Task NotifyShoutcasters(Competitor competitor)
        {
            var ourGuy = GetUserByCompetitor(competitor);
            if (ourGuy == null) return;

            var updates = new List<string?>();
            var updatesNoMentions = new List<string?>();

            var previous = await _databaseService.GetSecondLatestMeasurementAsync(competitor);
            var now = await _databaseService.GetLatestMeasurementAsync(competitor);

            var competitors = new List<Competitor>
            {
                Competitor.Addymer,
                Competitor.Alex,
                Competitor.Blu,
                Competitor.Brayden,
                Competitor.Cbri,
                Competitor.Eunora,
                Competitor.Paul
            };

            competitors.Remove(competitor);

            if (previous == null || now == null) return;

            foreach (var player in competitors)
            {
                var other = await _databaseService.GetLatestMeasurementAsync(player);
                if (other == null) continue;

                var playerName = GetUserByCompetitor(player);
                if (playerName == null) continue;

                updates.Add(GetVoiceline(previous.Strength, now.Strength, other.Strength, playerName.Mention, "is now stronger", "is now weaker", false));
                updates.Add(GetVoiceline(previous.Endurance, now.Endurance, other.Endurance, playerName.Mention, "now has more endurance", "now has less endurance", false));
                updates.Add(GetVoiceline(previous.Agility, now.Agility, other.Agility, playerName.Mention, "is now faster", "is now slower", false));
                updates.Add(GetVoiceline(previous.Overall, now.Overall, other.Overall, playerName.Mention, "beat", "lose to", true));

                updatesNoMentions.Add(GetVoiceline(previous.Strength, now.Strength, other.Strength, playerName.DisplayName, "is now stronger", "is now weaker", false));
                updatesNoMentions.Add(GetVoiceline(previous.Endurance, now.Endurance, other.Endurance, playerName.DisplayName, "now has more endurance", "now has less endurance", false));
                updatesNoMentions.Add(GetVoiceline(previous.Agility, now.Agility, other.Agility, playerName.DisplayName, "is now faster", "is now slower", false));
                updatesNoMentions.Add(GetVoiceline(previous.Overall, now.Overall, other.Overall, playerName.DisplayName, "beat", "lose to", true));
            }

            updates = updates.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            updatesNoMentions = updatesNoMentions.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            updates.Sort();
            updatesNoMentions.Sort();

            var embed = new EmbedBuilder();

            embed.Title = "I CAN'T BELIEVE MY EYES!";
            embed.Description = $"What a play from {ourGuy.Mention}!";

            var playCount = 0;
            foreach (var line in updates)
            {
                if (line == null) continue;
                embed.Description += $"\n- {line}";
                playCount++;
            }

            if (playCount < 1) return;

            embed.Color = new Color(178, 152, 220);
            embed.ThumbnailUrl = "https://i.imgur.com/WQ8jdvQ.png";

            var builtEmbed = embed.Build();
            await SendEmbedToChannelAsync(builtEmbed, _config.ChannelPorkOffGeneralId);


            var embed2 = new EmbedBuilder();

            embed2.Title = "I CAN'T BELIEVE MY EYES!";
            embed2.Description = $"What a play from {ourGuy.DisplayName}!";

            var playCount2 = 0;
            foreach (var line in updatesNoMentions)
            {
                if (line == null) continue;
                embed2.Description += $"\n- {line}";
                playCount2++;
            }

            if (playCount2 < 1) return;

            embed2.Color = new Color(178, 152, 220);
            embed2.ThumbnailUrl = "https://i.imgur.com/WQ8jdvQ.png";

            var builtEmbed2 = embed2.Build();

            await SendEmbedToChannelAsync(builtEmbed2, _config.ChannelMatchHistoryId);
        }

        private string? GetVoiceline(double previous, double now, double other, string otherName, string better, string worse, bool isOverall)
        {
            if (previous >= other && now < other)
                return isOverall ? $"He would now {worse} {otherName} in a fight!" : $"He {worse} than {otherName}!";

            if (previous <= other && now > other)
                return isOverall ? $"He would now {better} {otherName} in a fight!" : $"He {better} than {otherName}!";

            return null;
        }







        private int GetRankFromRoleName(string roleName)
        {
            if (roleName.StartsWith("👑")) return 1;
            if (roleName.StartsWith("2️⃣")) return 2;
            if (roleName.StartsWith("3️⃣")) return 3;
            if (roleName.StartsWith("4️⃣")) return 4;
            if (roleName.StartsWith("5️⃣")) return 5;
            if (roleName.StartsWith("6️⃣")) return 6;
            if (roleName.StartsWith("❌")) return 7;

            return -1;
        }

        private ulong UserIdToPorkerId(ulong userId)
        {
            if (userId == _config.PaulDiscordId)
                return _config.PaulPorkerId;

            if (userId == _config.AddymerDiscordId)
                return _config.AddymerPorkerId;

            if (userId == _config.EunoraDiscordId)
                return _config.EunoraPorkerId;

            if (userId == _config.BluDiscordId)
                return _config.BluPorkerId;

            if (userId == _config.AlexDiscordId)
                return _config.AlexPorkerId;

            if (userId == _config.BraydenDiscordId)
                return _config.BraydenPorkerId;

            if (userId == _config.CbriDiscordId)
                return _config.CbriPorkerId;

            return 0;
        }

        private ulong PorkerIdToUserId(ulong userId)
        {
            if (userId == _config.PaulPorkerId)
                return _config.PaulDiscordId;

            if (userId == _config.AddymerPorkerId)
                return _config.AddymerDiscordId;

            if (userId == _config.EunoraPorkerId)
                return _config.EunoraDiscordId;

            if (userId == _config.BluPorkerId)
                return _config.BluDiscordId;

            if (userId == _config.AlexPorkerId)
                return _config.AlexDiscordId;

            if (userId == _config.BraydenPorkerId)
                return _config.BraydenDiscordId;

            if (userId == _config.CbriPorkerId)
                return _config.CbriDiscordId;

            return 0;
        }

        private async Task UpdateRoles(IEnumerable<KeyValuePair<SocketGuildUser, double>> scores)
        {
            var guild = _client.GetGuild(_config.GuildPorkOffId);

            var roles = new List<ulong>
            {
                _config.Rank1RoleId,
                _config.Rank2RoleId,
                _config.Rank3RoleId,
                _config.Rank4RoleId,
                _config.Rank5RoleId,
                _config.Rank6RoleId,
                _config.Rank7RoleId
            };

            var users = new List<ulong>
            {
                _config.PaulDiscordId,
                _config.AddymerDiscordId,
                _config.EunoraDiscordId,
                _config.AlexDiscordId,
                _config.BluDiscordId,
                _config.BraydenDiscordId,
                _config.CbriDiscordId
            };

            var porkers = new List<ulong>
            {
                _config.PaulPorkerId,
                _config.AddymerPorkerId,
                _config.EunoraPorkerId,
                _config.AlexPorkerId,
                _config.BluPorkerId,
                _config.BraydenPorkerId,
                _config.CbriPorkerId
            };

            var lastRank = new Dictionary<ulong, int>();
            foreach (var (user, score) in scores)
            {
                if (user == null) continue;

                var porkerId = UserIdToPorkerId(user.Id);
                var porker = guild.GetUser(porkerId);

                if (porker == null) continue;

                var rank = porker.Roles.FirstOrDefault(x => x.Name.Contains("(")); // unbelievable hack
                if (rank == null) continue;
                var intRank = GetRankFromRoleName(rank.Name); 

                if (intRank > 0)
                    lastRank.Add(user.Id, intRank);
            }

            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    await RemoveRoleAsync(_config.GuildPorkOffId, user, role);
                }
            }

            foreach (var porker in porkers)
            {
                foreach (var role in roles)
                {
                    await RemoveRoleAsync(_config.GuildPorkOffId, porker, role);
                }
            }


            var ranks = new List<SocketRole?>
            {
                guild?.GetRole(_config.Rank1RoleId),
                guild?.GetRole(_config.Rank2RoleId),
                guild?.GetRole(_config.Rank3RoleId),
                guild?.GetRole(_config.Rank4RoleId),
                guild?.GetRole(_config.Rank5RoleId),
                guild?.GetRole(_config.Rank6RoleId),
                guild?.GetRole(_config.Rank7RoleId)
            };

            var i = 0;
            foreach (var (user, score) in scores)
            {
                if (ranks == null || ranks[i] == null) continue;

                var displayName = user.DisplayName;
                if (displayName == "THE HOGFATHER") displayName = "paul";

                var change = string.Empty;
                if (lastRank.ContainsKey(user.Id))
                {
                    var userLastRank = lastRank[user.Id];
                    var rankChange = userLastRank - (i + 1);

                    var isBad = rankChange < 0;
                    rankChange = Math.Abs(rankChange);

                    for (var x = 0; x < rankChange; x++)
                    {
                        change += isBad ? "🔽" : "🔼";
                    }
                }

                await ranks[i].ModifyAsync(r => r.Name = $"{NumberToEmoji(i+1)}{change} {displayName} ({score:F0})");
                //await user.AddRoleAsync(ranks[i]);

                if (user.Id == _config.PaulDiscordId)
                {
                    var porker = guild?.GetUser(_config.PaulPorkerId);
                    if (porker != null)
                        await porker.AddRoleAsync(ranks[i]);
                }

                if (user.Id == _config.AddymerDiscordId)
                {
                    var porker = guild?.GetUser(_config.AddymerPorkerId);
                    if (porker != null)
                        await porker.AddRoleAsync(ranks[i]);
                }

                if (user.Id == _config.EunoraDiscordId)
                {
                    var porker = guild?.GetUser(_config.EunoraPorkerId);
                    if (porker != null)
                        await porker.AddRoleAsync(ranks[i]);
                }

                if (user.Id == _config.BluDiscordId)
                {
                    var porker = guild?.GetUser(_config.BluPorkerId);
                    if (porker != null)
                        await porker.AddRoleAsync(ranks[i]);
                }

                if (user.Id == _config.AlexDiscordId)
                {
                    var porker = guild?.GetUser(_config.AlexPorkerId);
                    if (porker != null)
                        await porker.AddRoleAsync(ranks[i]);
                }

                if (user.Id == _config.BraydenDiscordId)
                {
                    var porker = guild?.GetUser(_config.BraydenPorkerId);
                    if (porker != null)
                        await porker.AddRoleAsync(ranks[i]);
                }

                if (user.Id == _config.CbriDiscordId)
                {
                    var porker = guild?.GetUser(_config.CbriPorkerId);
                    if (porker != null)
                        await porker.AddRoleAsync(ranks[i]);
                }

                i++;
            }
        }

        private string NumberToEmoji(int number)
        {
            if (number == 1) return "👑";
            if (number == 2) return "2️⃣";
            if (number == 3) return "3️⃣";
            if (number == 4) return "4️⃣";
            if (number == 5) return "5️⃣";
            if (number == 6) return "6️⃣";
            if (number == 7) return "❌";

            return "🐷";
        }

        public async Task<SocketGuildUser?> AddRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            var guild = _client.GetGuild(guildId);
            var user = guild?.GetUser(userId);
            var role = guild?.GetRole(roleId);

            if (user != null && role != null)
            {
                await user.AddRoleAsync(role);
                return user;
            }

            return null;
        }

        // Method to remove a role from a user
        public async Task<SocketGuildUser?> RemoveRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            var guild = _client.GetGuild(guildId);
            var user = guild?.GetUser(userId);
            var role = guild?.GetRole(roleId);

            if (user != null && role != null)
            {
                await user.RemoveRoleAsync(role);
                return user;
            }

            return null;
        }


        private async Task<Dictionary<SocketGuildUser, MeasurementDto>> GetPeople()
        {
            var people = new Dictionary<SocketGuildUser, MeasurementDto>();

            var paulLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Paul);
            var addymerLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Addymer);
            var alexLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Alex);
            var eunoraLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Eunora);
            var bluLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Blu);
            var braydenLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Brayden);
            var cbriLatest = await _databaseService.GetLatestMeasurementAsync(Competitor.Cbri);

            var paulUser = GetUserByCompetitor(Competitor.Paul);
            var addymerUser = GetUserByCompetitor(Competitor.Addymer);
            var alexUser = GetUserByCompetitor(Competitor.Alex);
            var eunoraUser = GetUserByCompetitor(Competitor.Eunora);
            var bluUser = GetUserByCompetitor(Competitor.Blu);
            var braydenUser = GetUserByCompetitor(Competitor.Brayden);
            var cbriUser = GetUserByCompetitor(Competitor.Cbri);

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

            if (braydenLatest != null && braydenUser != null)
            {
                braydenLatest.Height = 178;
                people.Add(braydenUser, braydenLatest);
            }

            if (cbriLatest != null && cbriUser != null)
            {
                cbriLatest.Height = 188;
                people.Add(cbriUser, cbriLatest);
            }

            return people;
        }

        public SocketGuildUser? GetUserByCompetitor(Competitor competitor)
        {
            var guild = _client.GetGuild(_config.GuildPorkOffId);
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

            if (competitor == Competitor.Brayden)
                user = guild?.GetUser(_config.BraydenDiscordId);

            if (competitor == Competitor.Cbri)
                user = guild?.GetUser(_config.CbriDiscordId);

            return user;
        }

        public async Task UpdateLeaderboard(Competitor competitor)
        {
            var leaderboardChannel = _client.GetGuild(_config.GuildPorkOffId).GetChannel(_config.ChannelLeaderboardId);
            var channel = leaderboardChannel as ITextChannel;
            if (channel == null) return;

            var messages = await channel.GetMessagesAsync(5).FlattenAsync();
            await channel.DeleteMessagesAsync(messages);

            await SendStrengthRanking();
            await SendEnduranceRanking();
            await SendAgilityRanking();
            await SendOverallRanking();

            await NotifyShoutcasters(competitor);
        }

        public async Task<IEnumerable<ulong?>> SendCompetitorUpdate(MeasurementDto measurement, Competitor competitor, MeasurementDto? lastKnownMeasurement)
        {
            if (measurement == null) return new List<ulong?>();

            if (lastKnownMeasurement == null)
                lastKnownMeasurement = new MeasurementDto
                {
                    Strength = 0,
                    Endurance = 0,
                    Agility = 0,
                    Overall = 0,
                };

            SocketGuildUser? user = GetUserByCompetitor(competitor);
            if (user == null) return null;

            var embed = new EmbedBuilder();
            var displayName = user.DisplayName;
            if (displayName == "THE HOGFATHER")
                displayName = "paul";

            embed.Title = $"{displayName.ToUpperInvariant()} WEIGHED-IN";

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
            eloField.Value = $"`{measurement.Strength:F0}`\n`{measurement.Endurance:F0}`\n`{measurement.Agility:F0}`\n`{measurement.Overall:F0}`";

            var strengthDelta = $"{(measurement.Strength - lastKnownMeasurement.Strength < 0 ? "" : "+")}{measurement.Strength - lastKnownMeasurement.Strength:F0}";
            var enduranceDelta = $"{(measurement.Endurance - lastKnownMeasurement.Endurance < 0 ? "" : "+")}{measurement.Endurance - lastKnownMeasurement.Endurance:F0}";
            var agilityDelta = $"{(measurement.Agility - lastKnownMeasurement.Agility < 0 ? "" : "+")}{measurement.Agility - lastKnownMeasurement.Agility:F0}";
            var overallDelta = $"{(measurement.Overall - lastKnownMeasurement.Overall < 0 ? "" : "+")}{measurement.Overall - lastKnownMeasurement.Overall:F0}";

            deltaField.Value = $"`{strengthDelta}`\n`{enduranceDelta}`\n`{agilityDelta}`\n`{overallDelta}`";

            embed.AddField(nameField);
            embed.AddField(eloField);
            embed.AddField(deltaField);

            embed.ThumbnailUrl = user.GetDisplayAvatarUrl();
            embed.ThumbnailUrl = embed.ThumbnailUrl.Replace(".gif", ".png");

            embed.Color = new Color(129, 131, 132);
            if (measurement.Overall - lastKnownMeasurement.Overall > 0)
                embed.Color = new Color(31, 139, 76);
            else if (measurement.Overall - lastKnownMeasurement.Overall < 0)
                embed.Color = new Color(237, 66, 69);

            var builtOverallEmbed = embed.Build();

            var result = new List<ulong?>
            {
                await SendEmbedAndCreateThread(builtOverallEmbed, _config.ChannelPorkOffGeneralId, competitor),
                await SendEmbedAndCreateThread(builtOverallEmbed, _config.ChannelMatchHistoryId, competitor)
            };

            return result;
        }

        private async Task<ulong?> SendEmbedAndCreateThread(Embed builtOverallEmbed, ulong channelId, Competitor competitor)
        {
            var message = await SendEmbedToChannelAsync(builtOverallEmbed, channelId);
            if (message == null || message.Channel is not ITextChannel channel) return null;

            var day = await _databaseService.GetDaysSinceFirstMeasurementAsync();
            var name = GetUserByCompetitor(competitor)?.DisplayName;

            if (name == "THE HOGFATHER")
                name = "paul";

            if (name == null) return null;

            var threadName = $"DAY {day}: {name}";
            var autoArchiveDuration = ThreadArchiveDuration.OneDay;

            var thread = await channel.CreateThreadAsync(
                threadName,
                ThreadType.PublicThread,
                autoArchiveDuration,
                message
            );

            return thread?.Id;
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
            nameField.Value = string.Join("\n", scores.Select((kvp, index) =>
            {
                var displayName = kvp.Key.DisplayName;
                if (displayName == "THE HOGFATHER")
                    displayName = "paul";

                return $"`{displayName}`";
            })); // \t\t{(int)Math.Round(kvp.Value)}\t{CharacterRanking.GetGrade((int)Math.Round(kvp.Value))}"));
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
                var topUser = scores.Select((kvp, index) => kvp.Key).FirstOrDefault();

                //embed.Color = new Color(154, 183, 217);
                embed.Color = new Color(240, 210, 170);
                embed.ThumbnailUrl = scores.Select((kvp, index) => topUser?.GetDisplayAvatarUrl().Replace(".gif", ".png")).FirstOrDefault() ?? "https://i.imgur.com/hgX2vDP.png";

                if (topUser?.DisplayName == "THE HOGFATHER" || topUser?.DisplayName == "paul")
                    embed.ThumbnailUrl = "https://i.imgur.com/48wZQBA.png";

                embed.ImageUrl = "https://i.imgur.com/B6qpFvU.png";
            }

            var builtEmbed = embed.Build();
            var message = await SendEmbedToChannelAsync(builtEmbed, _config.ChannelLeaderboardId);

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
