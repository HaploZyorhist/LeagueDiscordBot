#nullable enable
using Discord.Commands;
using Discord.WebSocket;
using LeagueDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactivity;
using LeagueDiscordBot.Modules;

namespace LeagueDiscordBot.Commands
{
    /// <summary>
    /// Help command for getting information on other commands
    /// </summary>
    [Group("Help")]
    [Summary("This funciton is used to give players information on how to use the bot")]
    public class Help : ModuleBase
    {
        #region Fields

        private readonly CommandService _commands;
        private LogService _logs;
        #endregion

        #region CTOR

        public Help(CommandService commands, LogService logs)
        {
            _commands = commands;
            _logs = logs;
        }

        #endregion

        #region Commands

        /// <summary>
        /// This command will give the user information about the commands
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns>information about commands</returns>
        [Command]
        [Summary("Allows players to get information about commands")]
        public async Task HelpCommand([Remainder] string? instruction = "")
        {
            // setup basic variables needed for the bot
            instruction = instruction?.ToLower();
            Discord.EmbedBuilder embedBuilder = new Discord.EmbedBuilder();

            // build list of commands
            List<CommandInfo> commandsList = _commands.Commands.ToList();

            // build dictionary of command groups
            Dictionary<string, string?> groups = new Dictionary<string, string?>();
            foreach (CommandInfo command in commandsList)
            {
                if (command.Module.Group != "Help" && !groups.ContainsKey(command.Module.Group))
                {
                    groups.TryAdd(command.Module.Group.ToLower(), command.Module.Summary);
                }
            }

            if (string.IsNullOrEmpty(instruction))
            {
                foreach (var key in groups)
                {
                    embedBuilder.AddField(key.Key, key.Value ?? "summary not set");
                }
            }
            else if (groups.ContainsKey(instruction))
            {
                foreach (var command in commandsList)
                {
                    if (command.Module.Group.ToLower() == instruction)
                    {
                        embedBuilder.AddField(command.Name ?? "Base", command.Summary ?? "summary not set");
                    }
                }
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }

        #endregion
    }
}
