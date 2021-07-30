#nullable enable 
using Discord.Commands;
using Interactivity;
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
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Generating list of champions for user",
                    TriggerChannel = Context.Channel.Name,
                    TriggerServer = Context.Guild.Name,
                    TriggerClass = nameof(Inventory),
                    TriggerFunction = nameof(GetOwnedChampsCommand),
                    User = ulong.Parse(user.Id.ToString())
                };

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
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerChannel = Context.Channel.Name,
                    TriggerServer = Context.Guild.Name,
                    TriggerClass = nameof(Inventory),
                    TriggerFunction = nameof(GetOwnedChampsCommand),
                    User = ulong.Parse(user.Id.ToString())
                };
            }
        }

        /// <summary>
        /// gets details of an owned champ by id
        /// </summary>
        [Command("Champ")]
        [Summary("Returns the details of a champion by id")]
        public async Task GetChampByID([Remainder] string? instruction = "")
        {
            var user = Context.User;
            try
            {
                string champion = instruction.Trim().ToLower();

                var champDetails = await _champService.GetChampByName(champion);

                if (champDetails == null)
                {
                    throw new Exception("The champion name you entered does not exist");
                }

                var champ = await _champService.GetMyChampByID(user, champDetails.ChampionID);

                if(champ == null)
                {
                    throw new Exception("The champion you're attemting to search for does not exist");
                }

                await ReplyAsync($"The champion you searched for is {champDetails.ChampionName}, Level {champ.ChampionLevel}");
            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerChannel = Context.Channel.Name,
                    TriggerServer = Context.Guild.Name,
                    TriggerClass = nameof(Inventory),
                    TriggerFunction = nameof(GetChampByID),
                    User = ulong.Parse(user.Id.ToString())
                };

                await ReplyAsync(ex.Message);
            }
        }

        #endregion
    }
}
