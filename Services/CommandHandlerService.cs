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

            string input = null;
            string command = null;
            string instruction = null;

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

            //if(message.Source != Discord.MessageSource.Bot && message.Content.StartsWith(prefix))
            //{
            //    try
            //    {
            //        input = message.Content.Remove(0, prefix.Length);
            //        command = input.Split(" ")[0];
            //        instruction = input.Remove(0, command.Length).Trim();

            //        if (string.IsNullOrEmpty(command))
            //        {
            //            return;
            //        }

            //        // a test method, will be removed
            //        if (command.ToLower() == "ping")
            //        {
            //            Ping(message, instruction);
            //        }

            //        // Method for setting custom prefixes for the bot
            //        if (message.Content.ToLower().Contains("!prefix") || 
            //            command.ToLower() == "prefix")
            //        {
            //            Prefix(message, instruction);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        var log = new LogMessage
            //        {
            //            Severity = Discord.LogSeverity.Error,
            //            Message = ex.Message,
            //            SourceClass = nameof(CommandHandlerService),
            //            SourceMethod = nameof(MessageReceived)
            //        };

            //        await _logs.ManualLog(log);
            //        await message.Channel.SendMessageAsync($"{command} failed");
            //    }
            //}
        }

        #endregion

        #region Commands

        /// <summary>
        /// Ping test command tree
        /// </summary>
        /// <param name="instruction"></param>
        public async void Ping(SocketMessage message, string instruction)
        {

        }

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
