using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueDiscordBot.DBTables
{
    /// <summary>
    /// data for the logs table
    /// </summary>
    public class LoLBotLogs
    {
        /// <summary>
        /// primary key for the log
        /// </summary>
        public int LogID { get; set; }

        /// <summary>
        /// id for the user 
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// date of the log
        /// </summary>
        public DateTime LogDate { get; set; }

        /// <summary>
        /// class that triggered log
        /// </summary>
        public string TriggerClass { get; set; }

        /// <summary>
        /// function that triggered log
        /// </summary>
        public string TriggerFunction { get; set; }

        /// <summary>
        /// method that triggered log
        /// </summary>
        public string TriggerMethod { get; set; }

        /// <summary>
        /// severity of the log
        /// </summary>
        public Discord.LogSeverity Severity { get; set; }

        /// <summary>
        /// message of the log
        /// </summary>
        public string LogMessage { get; set; }
    }
}
