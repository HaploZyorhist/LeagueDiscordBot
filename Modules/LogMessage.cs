using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueDiscordBot.Modules
{
    class LogMessage
    {
        public Discord.LogSeverity Severity { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}
