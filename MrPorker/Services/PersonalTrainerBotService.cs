using Discord;
using Discord.WebSocket;
using MrPorker.Configs;
using MrPorker.Data.Dtos;
using MrPorker.Data.Enums;
using System;

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
                GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions,
                AlwaysDownloadUsers = true // Enable user caching
            });
        }

        public async Task RunAsync()
        {
            _client.Ready += OnReadyAsync;
            _client.ReactionAdded += OnReactionAddedAsync;

            await _client.LoginAsync(TokenType.Bot, _config.PersonalTrainerBotToken);
            await _client.StartAsync();
        }

        private async Task OnReadyAsync()
        {
            await SendMessageToChannelAsync("there will be consequences for your absence.", _config.ChannelGeneralId);
        }

        private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction socketReaction)
        {
            if (socketReaction.Emote.Name != "❌") return;

            var guild = _client.GetGuild(_config.GuildHideoutId);
            if (cachedChannel.Id != _config.ChannelPorkCentralId) return;

            var channel = guild?.GetChannel(cachedChannel.Id) as ISocketMessageChannel;
            if (channel == null) return;

            var message = await channel.GetMessageAsync(cachedMessage.Id);
            
            int xCount = 0;
            foreach (var reaction in message.Reactions)
            {
                if (reaction.Key.Name == "⁉")
                {
                    var users = await message.GetReactionUsersAsync(reaction.Key, 20).FlattenAsync();
                    if (users.Any(user => user.Id == _client.CurrentUser.Id)) return;
                }

                if (reaction.Key.Name == "❌")
                {
                    var users = await message.GetReactionUsersAsync(reaction.Key, 20).FlattenAsync();
                    foreach (var reactionUser in users)
                    {
                        var socketUser = guild?.GetUser(reactionUser.Id);
                        if (socketUser == null || socketUser.Id == message.Author.Id || !socketUser.Roles.Any(role => role.Id == _config.ParticipantRoleId)) continue;

                        xCount++;
                    }
                }
            }

            if (xCount > 2)
            {
                var userMessage = message as IUserMessage;
                if (userMessage == null) return;

                var target = await RemoveRoleAsync(_config.GuildHideoutId, userMessage.Author.Id, _config.TalkerRoleId);
                if (target != null)
                {
                    var embed = new EmbedBuilder();
                    embed.Title = $"That's just not on.";
                    embed.Description = $"Nah {target.DisplayName}, that was fucked. You're out. Come back tomorrow.";

                    embed.Color = new Color(237, 66, 69);
                    var builtEmbed = embed.Build();

                    await userMessage.AddReactionAsync(new Emoji("⁉"));
                    await userMessage.ReplyAsync(embed: builtEmbed);
                }
            }
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

            await AddRoleAsync(_config.GuildHideoutId, _config.PaulDiscordId, _config.TalkerRoleId);
            await AddRoleAsync(_config.GuildHideoutId, _config.AddymerDiscordId, _config.TalkerRoleId);
            await AddRoleAsync(_config.GuildHideoutId, _config.AlexDiscordId, _config.TalkerRoleId);
            await AddRoleAsync(_config.GuildHideoutId, _config.EunoraDiscordId, _config.TalkerRoleId);
            await AddRoleAsync(_config.GuildHideoutId, _config.BluDiscordId, _config.TalkerRoleId);
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
            if (HasRole(_config.GuildHideoutId, participantId, _config.TalkerRoleId) && measurementDto == null)
                await RemoveRoleAsync(_config.GuildHideoutId, participantId, _config.TalkerRoleId);
            else if (!HasRole(_config.GuildHideoutId, participantId, _config.TalkerRoleId) && measurementDto != null)
                await AddRoleAsync(_config.GuildHideoutId, participantId, _config.TalkerRoleId);

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
