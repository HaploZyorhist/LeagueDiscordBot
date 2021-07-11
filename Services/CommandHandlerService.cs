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
    /// <summary>
    /// service class for handling commands
    /// </summary>
    public class CommandHandlerService
    {

        #region Fields

        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private IServiceProvider _provider;
        private LogService _logs;

        //const client = new Discord.Client();

        #endregion

        #region CTOR

        /// <summary>
        /// CTOR for class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="provider"></param>
        /// <param name="commands"></param>
        /// <param name="logs"></param>
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

        #endregion

        #region CommandHandler

        /// <summary>
        /// Task for setting up the class
        /// </summary>
        /// <param name="provider"></param>
        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        /// <summary>
        /// Message handler, should manage the commands
        /// </summary>
        /// <param name="message"></param>
        private async Task MessageReceived(SocketMessage message)
        {
            var prefix = Environment.GetEnvironmentVariable("prefix");

            if (!(message is SocketUserMessage socketMessage))
            {
                return;
            }

            if (message.Source != Discord.MessageSource.User)
            {
                return;
            }

            if (!(message.ToString().StartsWith(prefix)))
            {
                return;
            }

            var context = new SocketCommandContext(_client, socketMessage);

            await _commands.ExecuteAsync(context, prefix.Length, _provider);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command for changing the prefix of the bot
        /// </summary>
        /// <param name="message"></param>
        /// <param name="instruction"></param>
        public async void Prefix(SocketMessage message, string instruction)
        {
            string newPrefix = "";
            try
            {
                if (instruction.Length > 0 && instruction.Length < 4)
                {
                    newPrefix = instruction;

                    var log = new LogMessage
                    {
                        Severity = Discord.LogSeverity.Info,
                        Message = "Prefix was reset",
                        SourceClass = nameof(CommandHandlerService),
                        SourceMethod = nameof(Prefix)
                    };

                    Environment.SetEnvironmentVariable("prefix", newPrefix);

                    await _logs.ManualLog(log);
                    await message.Channel.SendMessageAsync($"your prefix was changed to {newPrefix}");
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
