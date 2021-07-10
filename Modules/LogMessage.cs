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
        /// initiating method
        /// </summary>
        public string SourceMethod { get; set; }

        /// <summary>
        /// initiating class
        /// </summary>
        public string SourceClass { get; set; }

        /// <summary>
        /// user who triggered the log
        /// </summary>
        public string User { get; set; }
    }
}
