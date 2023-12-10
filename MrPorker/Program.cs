﻿using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrPorker;
using MrPorker.Configs;
//using MrPorker.Services;
using System;
using System.Threading.Tasks;
using MrPorker.Services;

Console.WriteLine("Hello, World!");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = ConfigureServices(configuration);

var serviceProvider = services.BuildServiceProvider();
var bot = serviceProvider.GetRequiredService<Bot>();
await bot.RunAsync();

IServiceCollection ConfigureServices(IConfiguration configuration)
{
    var botConfig = configuration.Get<BotConfig>() ?? throw new InvalidOperationException("Bot configuration not found.");

    return new ServiceCollection()
        .AddSingleton(new DiscordSocketClient())
        .AddSingleton<InteractionService>()
        .AddSingleton<CommandHandler>()
        .AddSingleton(botConfig)
        .AddSingleton<Bot>();
}