using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MrPorker.Configs;
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
            //await interactionService.AddCommandsGloballyAsync();
            await interactionService.RegisterCommandsToGuildAsync(config.GuildPorkOffId);
            //await SendMessageToChannelAsync("good heavens", config.ChannelGeneralId);


            // GOOD SERVER: 
            //await interactionService.RegisterCommandsToGuildAsync(config.GuildPorkerId);
        }

        private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(client, interaction);
            await interactionService.ExecuteCommandAsync(ctx, services);
        }

        private async Task OnMessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return; // Ignore bot's own messages

            if (message.Content.ToLower().Contains("hello"))
                await message.Channel.SendMessageAsync("Hello!");

            var twitterLink = ReplaceTwitterLinks(message.Content);
            if (twitterLink != null)
            {
                if (message is SocketUserMessage userMessage)
                {
                    var mentionString = userMessage.Author.Mention;
                    await userMessage.DeleteAsync();
                    await message.Channel.SendMessageAsync(mentionString + " sent: " + twitterLink, allowedMentions: AllowedMentions.None);
                }
            }
        }

        private string? ReplaceTwitterLinks(string message)
        {
            var match = Regex.Match(message, config.TwitterLinkRegex);
            return match.Success ? $"{config.TwitterLinkReplacementHost}{match.Groups[3].Value}" : null;
        }

        #endregion

        public async Task SendMessageToChannelAsync(string content, ulong channelId)
        {
            if (await client.GetChannelAsync(channelId) is IMessageChannel channel)
                await channel.SendMessageAsync(content);
            else
                Console.WriteLine($"Failed to send message to channel ID: {channelId}");
        }

        public async Task SendFileToChannelAsync(MemoryStream imageStream, ulong channelId)
        {
            if (await client.GetChannelAsync(channelId) is IMessageChannel channel)
                await channel.SendFileAsync(imageStream, "output.png");
            else
                Console.WriteLine($"Failed to send file to channel ID: {channelId}");
        }

        public async Task<IUserMessage?> SendEmbedToChannelAsync(Embed embed, ulong channelId, MessageComponent? components = null)
        {
            if (await client.GetChannelAsync(channelId) is IMessageChannel channel)
                return await channel.SendMessageAsync(embed: embed, components: components);
            else
                Console.WriteLine($"Failed to send file to channel ID: {channelId}");

            return null;
        }

        public async Task<IUserMessage?> SendImageEmbedToChannelAsync(MemoryStream imageStream, ulong threadId)
        {
            var thread = client.GetGuild(config.GuildPorkOffId).GetThreadChannel(threadId);
            if (thread == null) return null;

            var embed = new EmbedBuilder()
                .WithImageUrl("attachment://output.png")
                .WithColor(Color.Blue)
            .Build();

            return await thread.SendFileAsync(imageStream, "output.png", embed: embed);
        }

        public async Task<IUserMessage?> SendHeader(IThreadChannel thread)
        {
            var embed = new EmbedBuilder()
                .WithImageUrl("https://i.imgur.com/sF72DRf.png")
                .WithColor(Color.Blue)
            .Build();

            return await thread.SendMessageAsync(embed: embed);

            //if (await client.GetChannelAsync(channelId) is IMessageChannel channel)
            //{

            //}
            //else
            //{
            //    Console.WriteLine($"Failed to send file to channel ID: {channelId}");
            //}

            //return null;
        }

        public async Task<IUserMessage?> SendFileToThreadAsync(MemoryStream imageStream, ulong threadId)
        {
            var thread = client.GetGuild(config.GuildPorkOffId).GetThreadChannel(threadId);
            if (thread == null) return null;

            return await thread.SendFileAsync(imageStream, "output.png");
        }
    }
}
