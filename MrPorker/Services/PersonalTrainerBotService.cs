using Discord;
using Discord.WebSocket;
using MrPorker.Configs;
using MrPorker.Data.Dtos;
using MrPorker.Data.Enums;

namespace MrPorker.Services
{
    public class PersonalTrainerBotService
    {
        private readonly DatabaseService _databaseService;
        private readonly DiscordSocketClient _client;
        private readonly BotConfig _config;

        public PersonalTrainerBotService(BotConfig config, DatabaseService databaseService)
        {
            _databaseService = databaseService;

            _config = config;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.Guilds,
                AlwaysDownloadUsers = true // Enable user caching
            });
        }

        public async Task RunAsync()
        {
            _client.Ready += OnReadyAsync;
            await _client.LoginAsync(TokenType.Bot, _config.PersonalTrainerBotToken);
            await _client.StartAsync();
        }

        private async Task OnReadyAsync()
        {
            await SendMessageToChannelAsync("there will be consequences for your absence.", _config.ChannelGeneralId);
        }

        public async Task SendMessageToChannelAsync(string content, ulong channelId)
        {
            if (await _client.GetChannelAsync(channelId) is IMessageChannel channel)
                await channel.SendMessageAsync(content);
            else
                Console.WriteLine($"Failed to send message to channel ID: {channelId}");
        }

        // Method to add a role to a user
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

        public async Task GiveAllRoles()
        {
            await AddRoleAsync(_config.GuildHideoutId, _config.PaulDiscordId, _config.ParticipantRoleId);
            await AddRoleAsync(_config.GuildHideoutId, _config.AddymerDiscordId, _config.ParticipantRoleId);
            await AddRoleAsync(_config.GuildHideoutId, _config.AlexDiscordId, _config.ParticipantRoleId);
            await AddRoleAsync(_config.GuildHideoutId, _config.EunoraDiscordId, _config.ParticipantRoleId);
            await AddRoleAsync(_config.GuildHideoutId, _config.BluDiscordId, _config.ParticipantRoleId);
        }

        public async Task Judge()
        {
            var paulYesterday = await _databaseService.GetXthMostRecentMeasurementAsync(1, Competitor.Paul);
            var addymerYesterday = await _databaseService.GetXthMostRecentMeasurementAsync(1, Competitor.Addymer);
            var alexYesterday = await _databaseService.GetXthMostRecentMeasurementAsync(1, Competitor.Alex);
            var eunoraYesterday = await _databaseService.GetXthMostRecentMeasurementAsync(1, Competitor.Eunora);
            var bluYesterday = await _databaseService.GetXthMostRecentMeasurementAsync(1, Competitor.Blu);

            await Decree(paulYesterday, _config.PaulDiscordId);
            await Decree(addymerYesterday, _config.AddymerDiscordId);
            await Decree(alexYesterday, _config.AlexDiscordId);
            await Decree(eunoraYesterday, _config.EunoraDiscordId);
            await Decree(bluYesterday, _config.BluDiscordId);
        }

        private async Task Decree(MeasurementDto? measurementDto, ulong participantId)
        {
            if (HasRole(_config.GuildHideoutId, participantId, _config.ParticipantRoleId) && measurementDto == null)
            {
                var target = await RemoveRoleAsync(_config.GuildHideoutId, participantId, _config.ParticipantRoleId);
                if (target != null)
                {
                    var embed = new EmbedBuilder();
                    embed.Title = $"SUSPENSION";
                    embed.Description = $"{target.DisplayName} did not weigh in yesterday.";
                    embed.ThumbnailUrl = target.GetDisplayAvatarUrl();

                    embed.Color = new Color(237, 66, 69);
                    var builtEmbed = embed.Build();

                    await SendEmbedToChannelAsync(builtEmbed, _config.ChannelPorkCentralId);
                }
            }
            else if (!HasRole(_config.GuildHideoutId, participantId, _config.ParticipantRoleId) && measurementDto != null)
            {
                var target = await AddRoleAsync(_config.GuildHideoutId, participantId, _config.ParticipantRoleId);
                if (target != null)
                {
                    var embed = new EmbedBuilder();
                    embed.Title = $"WELCOME BACK";
                    embed.Description = $"Don't let me catch you slacking again, {target.DisplayName}.";
                    embed.ThumbnailUrl = target.GetDisplayAvatarUrl();

                    embed.Color = new Color(31, 139, 76);
                    var builtEmbed = embed.Build();

                    await SendEmbedToChannelAsync(builtEmbed, _config.ChannelPorkCentralId);
                }
            }
        }

        public bool HasRole(ulong guildId, ulong userId, ulong roleId)
        {
            var guild = _client.GetGuild(guildId);
            var user = guild?.GetUser(userId);

            if (user != null)
            {
                foreach (var role in user.Roles)
                {
                    if (role.Id == roleId)
                        return true;
                }
            }

            return false;
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
