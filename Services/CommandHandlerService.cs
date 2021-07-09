using Discord.Commands;
using Discord.WebSocket;
using LeagueDiscordBot.Modules;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Services
{
    public class CommandHandlerService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private IServiceProvider _provider;
        private LogService _logs;

        public CommandHandlerService(DiscordSocketClient client, 
                                     IServiceProvider provider, 
                                     CommandService commands,
                                     LogService logs)
        {
            _commands = commands;
            _client = client;
            _provider = provider;
            _logs = logs;

            _client.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
            //_client.Log += Log;
            //_provider = provider;
            //await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Ping was pressed",
                    Source = nameof(MessageReceived)
                };

                await _logs.Log(new Discord.LogMessage(log.Severity, log.Source, log.Message));

                await message.Channel.SendMessageAsync("Pong!");
            }

            if (message.Content == "!coding is fun")
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "coding message",
                    Source = nameof(MessageReceived)
                };

                await _logs.Log(new Discord.LogMessage(log.Severity, log.Source, log.Message));

                await message.Channel.SendMessageAsync("You're a fucking liar!");
            }
        }
    }
}
