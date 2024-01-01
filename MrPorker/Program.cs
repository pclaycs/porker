using Discord.WebSocket;
using Discord.Interactions;
using MrPorker.Configs;
using MrPorker.Services;
using Discord;
using MrPorker.Data;
using Microsoft.EntityFrameworkCore;
using MrPorker.Api;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("BotDatabase");
var botConfig = configuration.Get<BotConfig>() ?? throw new InvalidOperationException("Bot configuration not found.");

var builder = WebApplication.CreateSlimBuilder(args);
// Discord Bot Service
builder.Services.AddHttpClient();
builder.Services.AddSingleton(botConfig);
builder.Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
}));
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddSingleton<BotService>();
builder.Services.AddDbContext<BotDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<HoroscopeService>();

var app = builder.Build();

// Seed database
var databaseService = app.Services.GetRequiredService<DatabaseService>();
await databaseService.SeedDatabaseAsync();

// Initialize the Discord Bot
var botService = app.Services.GetRequiredService<BotService>();
await botService.RunAsync();

Router.ConfigureEndpoints(app);
app.Run();
