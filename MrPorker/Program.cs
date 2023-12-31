using Discord.WebSocket;
using Discord.Interactions;
using MrPorker.Configs;
using MrPorker.Services;
using Discord;
using MrPorker.Data;
using Microsoft.EntityFrameworkCore;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("BotDatabase");
var botConfig = configuration.Get<BotConfig>() ?? throw new InvalidOperationException("Bot configuration not found.");

var builder = WebApplication.CreateSlimBuilder(args);
// Discord Bot Service
builder.Services.AddHttpClient();
builder.Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
}));
builder.Services.AddSingleton(botConfig);
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<HoroscopeService>();
builder.Services.AddSingleton<BotService>();
builder.Services.AddDbContext<BotDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddSingleton<DatabaseService>();

var app = builder.Build();

// Seed database
var databaseService = app.Services.GetRequiredService<DatabaseService>();
await databaseService.SeedDatabaseAsync();

// Initialize the Discord Bot
var botService = app.Services.GetRequiredService<BotService>();
await botService.RunAsync();

var botApi = app.MapGroup("/bot");

botApi.MapGet("/ping", async (BotService bot) =>
{
    await bot.SendMessageAsync("pong");
    return Results.Ok();
});


app.Run();
