﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LeagueDiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LeagueDiscordBot
{
	public class Program
	{
		private DiscordSocketClient _client;

		public static Task Main(string[] args) => new Program().MainAsync();

		public async Task MainAsync()
		{
			_client = new DiscordSocketClient();

			var services = ConfigureServices();

			await services.GetRequiredService<CommandHandlerService>().InitializeAsync(services);
			
			var token = File.ReadAllText(@"..\..\..\token.json");
			await _client.LoginAsync(TokenType.Bot, token);
				//Environment.GetEnvironmentVariable("DiscordToken"));
			await _client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

        private IServiceProvider ConfigureServices()
		{
			return new ServiceCollection()
				// Base
				.AddSingleton(_client)
				.AddSingleton<CommandService>()
				.AddSingleton<CommandHandlerService>()

				// Logs
				.AddSingleton<LogService>()
				.BuildServiceProvider();
		}
	}
}
