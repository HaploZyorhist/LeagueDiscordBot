using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueDiscordBot.Modules
{
    /// <summary>
    /// Class containing log contents
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Severity of log
        /// </summary>
        public Discord.LogSeverity Severity { get; set; }

        /// <summary>
        /// Log Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// user who triggered the log
        /// </summary>
        public ulong? User { get; set; }

        /// <summary>
        /// class that triggered log
        /// </summary>
        public string TriggerClass { get; set; }

        /// <summary>
        /// function that triggered log
        /// </summary>
        public string TriggerFunction { get; set; }

        /// <summary>
        /// server that triggered log
        /// </summary>
        public string TriggerServer { get; set; }

        /// <summary>
        /// Channel that triggered log
        /// </summary>
        public string TriggerChannel { get; set; }
    }
}
