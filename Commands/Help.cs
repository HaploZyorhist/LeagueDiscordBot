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
        private readonly LogService _logs;

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
            try
            {
                // TODO: fix help construction
                // setup basic variables needed for the bot
                instruction = instruction?.ToLower();
                Discord.EmbedBuilder embedBuilder = new Discord.EmbedBuilder();

                // build list of commands
                List<CommandInfo> commandsList = _commands.Commands.ToList();

                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Initiating help command",
                    TriggerChannel = Context.Channel.Name,
                    TriggerServer = Context.Guild.Name,
                    TriggerClass = nameof(Help),
                    TriggerFunction = nameof(HelpCommand),
                    User = ulong.Parse(Context.User.Id.ToString())
                };

                // build dictionary of command groups
                Dictionary<string, string?> groups = new Dictionary<string, string?>();
                foreach (CommandInfo command in commandsList)
                {
                    if (command.Module.Group != "Help" && !groups.ContainsKey(command.Module.Group))
                    {
                        if (string.IsNullOrEmpty(command.Module.Group))
                        {
                            groups.TryAdd(command.Name.ToLower(), command.Summary);
                        }
                        else
                        {
                            groups.TryAdd(command.Module.Group.ToLower(), command.Module.Summary);
                        }
                    }
                }

                if (string.IsNullOrEmpty(instruction))
                {
                    log = new LogMessage
                    {
                        Severity = Discord.LogSeverity.Info,
                        Message = "Building list of help groups",
                        TriggerChannel = Context.Channel.Name,
                        TriggerServer = Context.Guild.Name,
                        TriggerClass = nameof(Help),
                        TriggerFunction = nameof(HelpCommand),
                        User = ulong.Parse(Context.User.Id.ToString())
                    };
                    foreach (var key in groups)
                    {
                        embedBuilder.AddField(key.Key, key.Value ?? "summary not set");
                    }
                }
                else if (groups.ContainsKey(instruction))
                {
                    log = new LogMessage
                    {
                        Severity = Discord.LogSeverity.Info,
                        Message = "Building list of commands",
                        TriggerChannel = Context.Channel.Name,
                        TriggerServer = Context.Guild.Name,
                        TriggerClass = nameof(Help),
                        TriggerFunction = nameof(HelpCommand),
                        User = ulong.Parse(Context.User.Id.ToString())
                    };
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
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = "Help command failed " + ex.Message,
                    TriggerChannel = Context.Channel.Name,
                    TriggerServer = Context.Guild.Name,
                    TriggerClass = nameof(Help),
                    TriggerFunction = nameof(HelpCommand),
                    User = ulong.Parse(Context.User.Id.ToString())
                };
            }
        }

        #endregion
    }
}
