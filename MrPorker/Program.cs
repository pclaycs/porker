﻿using Discord.WebSocket;
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
builder.Services.AddSingleton<EunoraBotService>();
builder.Services.AddSingleton<BluBotService>();
builder.Services.AddSingleton<BraydenBotService>();
builder.Services.AddSingleton<CbriBotService>();
builder.Services.AddDbContext<BotDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<HogHoganBotService>();
builder.Services.AddSingleton<HoroscopeService>();
builder.Services.AddSingleton<MeasurementService>();
builder.Services.AddSingleton<FirebaseService>();
builder.Services.AddSingleton<TimedMessagingService>();
builder.Services.AddSingleton<MailPollingService>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5277);
});

var app = builder.Build();

// Seed database
var databaseService = app.Services.GetRequiredService<DatabaseService>();
await databaseService.ApplyMigrations();
await databaseService.SeedDatabaseAsync();

// Initialize the Discord Bot
var botService = app.Services.GetRequiredService<BotService>();
await botService.RunAsync();

var addymerBotService = app.Services.GetRequiredService<AddymerBotService>();
await addymerBotService.RunAsync();

var alexBotService = app.Services.GetRequiredService<AlexBotService>();
await alexBotService.RunAsync();

var eunoraBotService = app.Services.GetRequiredService<EunoraBotService>();
await eunoraBotService.RunAsync();

var bluBotService = app.Services.GetRequiredService<BluBotService>();
await bluBotService.RunAsync();

var braydenBotService = app.Services.GetRequiredService<BraydenBotService>();
await braydenBotService.RunAsync();

var cbriBotService = app.Services.GetRequiredService<CbriBotService>();
await cbriBotService.RunAsync();

var hogHoganBotService = app.Services.GetRequiredService<HogHoganBotService>();
await hogHoganBotService.RunAsync();

var timedMessagingService = app.Services.GetRequiredService<TimedMessagingService>();
Task.Run(async () => await timedMessagingService.StartAsync(_cancellationTokenSource.Token));

var firebasePollingService = app.Services.GetRequiredService<FirebaseService>();
Task.Run(async () => await firebasePollingService.StartPollingAsync(_cancellationTokenSource.Token));

var mailPollingService = app.Services.GetRequiredService<MailPollingService>();
Task.Run(async () => await mailPollingService.StartPollingAsync(_cancellationTokenSource.Token));

// Used for convenient debugging with Postman, this blocks the Firebase Poller currently
Router.ConfigureEndpoints(app);
app.Run();

