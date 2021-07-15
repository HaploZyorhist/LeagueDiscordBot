using Discord.Commands;
using LeagueDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    /// <summary>
    /// Function for getting players out of their current interactions
    /// </summary>
    [Group("Cancel")]
    [Summary("This funciton should unlock a player from their interactions")]
    public class Cancel : ModuleBase
    {
        #region Fields

        private LockOutService _lock;
        private LogService _logs;

        #endregion

        #region CTOR

        public Cancel(LogService logs, LockOutService lockout)
        {
            _logs = logs;
            _lock = lockout;
        }

        #endregion

        #region Commands

        /// <summary>
        /// This command will unlock a player
        /// </summary>
        [Command]
        [Summary("Unlocks player from current command")]
        public async Task CancelCommand()
        {
            // TODO: add logging into cancel command
            try
            {
                var user = Context.User;

                await _lock.UnlockUser(user);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
