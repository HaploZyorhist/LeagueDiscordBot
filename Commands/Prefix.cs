using Discord.Commands;
using LeagueDiscordBot.Modules;
using LeagueDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    /// <summary>
    /// Prefix command for changing the prefix 
    /// </summary>
    [Group("Prefix")]
    [Summary("This funciton is used to change the prefix for the bot")]
    public class Prefix : ModuleBase
    {
        #region Fields

        private LogService _logs;

        #endregion

        #region CTOR

        public Prefix(LogService logs)
        {
            _logs = logs;
        }

        #endregion

        #region Prefix

        /// <summary>
        /// Command for setting the prefix for the bot
        /// </summary>
        /// <param name="instruction"></param>
        [Command]
        [Summary("This command sets the prefix for the bot.  It is administrator locked")]
        [RequireUserPermission(Discord.GuildPermission.Administrator, Group = "Permission")]
        public async Task PrefixCommand([Remainder] string instruction)
        {
            string newPrefix = "";
            var user = Context.User;
            try
            {
                if (instruction.Length > 0 && instruction.Length < 4)
                {
                    newPrefix = instruction;

                    var log = new LogMessage
                    {
                        Severity = Discord.LogSeverity.Info,
                        Message = "Prefix was reset",
                        TriggerClass = nameof(Prefix),
                        TriggerFunction = nameof(PrefixCommand)
                    };

                    Environment.SetEnvironmentVariable("prefix", newPrefix);

                    await _logs.ManualLog(log);
                    await ReplyAsync($"your prefix was changed to {newPrefix}");
                }

            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
