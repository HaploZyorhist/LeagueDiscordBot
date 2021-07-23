using LeagueDiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Services
{
    public class BattleService
    {
        #region Fields

        private readonly LockOutService _lockout;
        private readonly ChampionService _champ;
        private readonly LogService _log;

        #endregion

        #region CTOR

        public BattleService(LockOutService lockOut,
                             ChampionService champ,
                             LogService log)
        {
            _lockout = lockOut;
            _champ = champ;
            _log = log;
        }

        #endregion

        #region Battle

        public async Task<Discord.IUser> Battle (Discord.IUser player1, Discord.IUser player2, Discord.IMessageChannel channel)
        {
            string action = "Battling";
   
            try
            {
                await _lockout.LockUser(player1, action);
                await _lockout.LockUser(player2, action);

                await channel.SendMessageAsync("a battle has begun");

                return null;
            }
            catch(Exception ex)
            {
                await _log.ManualLog(new LogMessage
                {
                    User = ulong.Parse(player1.Id.ToString()),
                    TriggerChannel = channel.Name,
                    TriggerClass = nameof(BattleService),
                    TriggerFunction = nameof(Battle),
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message
                });

                await channel.SendMessageAsync("battle has ended");

                await _lockout.UnlockUser(player1);
                await _lockout.UnlockUser(player2);

                return null;
            }
        }

        #endregion
    }
}
