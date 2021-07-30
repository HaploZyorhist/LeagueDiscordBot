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
    /// Group of commands for the shop
    /// </summary>
    [Group("Shop")]
    [Summary("Commands for interacting with the shop")]
    public class Shop : ModuleBase
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
        public Shop(CommandService commands,
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
        /// gets details of a champ by id
        /// </summary>
        [Command("Champ")]
        [Summary("Returns the details of a champion by id")]
        public async Task GetChampByID([Remainder] string? instruction = "")
        {
            var user = Context.User;
            try
            {
                int champID = 0;
                if (int.TryParse(instruction, out int result))
                {
                    champID = result;
                }
                else
                {
                    throw new Exception("A number must be passed in for the champion id");
                }

                var champ = await _champService.GetChampByID(champID);

                if (champ == null)
                {
                    throw new Exception("The champion you're attemting to search for does not exist");
                }

                await ReplyAsync($"The champion you searched for is {champ.ChampionName}");
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