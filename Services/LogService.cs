using Discord.Commands;
using Discord.WebSocket;
using LeagueDiscordBot.DbContexts;
using LeagueDiscordBot.DBTables;
using LeagueDiscordBot.Modules;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
            LogMessage logConversion = new LogMessage
            {
                Severity = msg.Severity,
                TriggerClass = msg.Source,
                User = 1,
                Message = msg.Message
            };
            ManualLog(logConversion);
			return Task.CompletedTask;
		}

        /// <summary>
        /// log system for manually generated logs
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public Task ManualLog(LogMessage log)
        {
            try
            {
                // prepare the log to send into the table
                LoLBotLogs tableLog = new LoLBotLogs
                {
                    TriggerServer = log.TriggerServer ?? "Unknown",
                    TriggerChannel = log.TriggerChannel ?? "Unknown",
                    TriggerClass = log.TriggerClass ?? "Unknown",
                    TriggerFunction = log.TriggerFunction ?? "Unknown",
                    LogDate = DateTime.Now,
                    Severity = log.Severity.ToString(),
                    UserID = log.User ?? 2005,
                    LogMessage = log.Message
                };

                LoLBotContext db = new LoLBotContext();
                db.LoLBotLogs.Add(tableLog);
                db.SaveChanges();

                Console.WriteLine("Log was insereted into logging table");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging service could not insert into table " + ex.Message);
                return Task.CompletedTask;
            }
        }

        #endregion
    }
}
