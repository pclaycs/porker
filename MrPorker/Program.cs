using Discord.WebSocket;
using Discord.Interactions;
using MrPorker.Configs;
using MrPorker.Services;
using Discord;
using MrPorker.Data;
using Microsoft.EntityFrameworkCore;
using MrPorker.Api;
using MrPorker;

var _cancellationTokenSource = new CancellationTokenSource();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("BotDatabase");
var botConfig = configuration.Get<BotConfig>() ?? throw new InvalidOperationException("Bot configuration not found.");

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddHttpClient();
builder.Services.AddSingleton(botConfig);
builder.Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
}));
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddSingleton<BotService>();
builder.Services.AddSingleton<AddymerBotService>();
builder.Services.AddSingleton<AlexBotService>();
builder.Services.AddDbContext<BotDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<HoroscopeService>();
builder.Services.AddSingleton<MeasurementService>();
builder.Services.AddSingleton<FirebasePollingService>();
builder.Services.AddSingleton<TimedMessagingService>();

var app = builder.Build();

// Seed database
var databaseService = app.Services.GetRequiredService<DatabaseService>();
await databaseService.SeedDatabaseAsync();

// Initialize the Discord Bot
var botService = app.Services.GetRequiredService<BotService>();
await botService.RunAsync();

var addymerBotService = app.Services.GetRequiredService<AddymerBotService>();
await addymerBotService.RunAsync();

var alexBotService = app.Services.GetRequiredService<AlexBotService>();
await alexBotService.RunAsync();

var timedMessagingService = app.Services.GetRequiredService<TimedMessagingService>();
Task.Run(async () => await timedMessagingService.StartAsync(_cancellationTokenSource.Token));

var firebasePollingService = app.Services.GetRequiredService<FirebasePollingService>();
Task.Run(async () => await firebasePollingService.StartPollingAsync(_cancellationTokenSource.Token));

// Used for convenient debugging with Postman, this blocks the Firebase Poller currently
Router.ConfigureEndpoints(app);
app.Run();

