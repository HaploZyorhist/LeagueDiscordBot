using Discord.Commands;
using Interactivity;
using LeagueDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    /// <summary>
    /// Group of basic commands
    /// </summary>
    [Group("Inventory")]
    [Summary("Commands for managing player inventory")]
    public class Inventory : ModuleBase
    {
        #region Fields

        private readonly CommandService _commands;
        private readonly LogService _logs;
        private readonly LockOutService _lock;
        private readonly RegistrationService _register;
        private readonly InteractivityService _interact;
        private readonly ChampionService _champService;

        #endregion

        #region CTOR

        /// <summary>
        /// Ctor for basic commands
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="logs"></param>
        public Inventory(CommandService commands,
                             LogService logs,
                             LockOutService lockout,
                             RegistrationService register,
                             InteractivityService interact,
                             ChampionService champService)
        {
            _commands = commands;
            _logs = logs;
            _lock = lockout;
            _register = register;
            _interact = interact;
            _champService = champService;
        }

        #endregion

        #region Commands

        /// <summary>
        /// get all my owned champions
        /// </summary>
        [Command("Champions")]
        [Summary("Returns the list of champions that the player owns")]
        public async Task GetOwnedChampsCommand ()
        {
            var user = Context.User;
            try
            {
                var myChamps = await _champService.GetMyChamps(user);
                var tiers = await _champService.GetTierList();

                var responseSting = new StringBuilder();
                
                foreach(var champ in myChamps)
                {
                    var champData = await _champService.GetChampByID(champ.ChampionID);
                    var champTier = tiers.GetValueOrDefault(champ.ChampionTier);
                    responseSting.AppendLine($"{champData.ChampionName}, {champData.Series}, {champTier}, Level {champ.ChampionLevel}, Exp {champ.ChampionExp}");
                }

                await ReplyAsync(responseSting.ToString());
            }
            catch(Exception ex)
            {

            }
        }

        #endregion
    }
}
