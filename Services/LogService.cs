using Discord.Commands;
using Discord.WebSocket;
using LeagueDiscordBot.Modules;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Services
{
    /// <summary>
    /// Service class for logs
    /// </summary>
    public class LogService
    {

        #region Fields

        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        #endregion

        #region CTOR

        /// <summary>
        /// CTOR class for log service
        /// </summary>
        /// <param name="discord"></param>
        /// <param name="commands"></param>
        public LogService(DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;

            _discord.Log += AutoLog;
            _commands.Log += AutoLog;
        }

        #endregion

        #region Logs

        /// <summary>
        /// Log system for auto generated logs
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task AutoLog(Discord.LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

        /// <summary>
        /// log system for manually generated logs
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public Task ManualLog(LogMessage log)
        {
            Console.WriteLine(log.Severity + "\t" + log.User + "\t" + log.SourceClass + "\t" + log.SourceMethod + "\t\t" + log.Message);
            return Task.CompletedTask;
        }

        #endregion
    }
}
