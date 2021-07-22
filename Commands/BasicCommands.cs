using Discord.Commands;
using Interactivity;
using LeagueDiscordBot.DBTables;
using LeagueDiscordBot.Modules;
using LeagueDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    /// <summary>
    /// Group of basic commands
    /// </summary>
    [Group("")]
    [Summary("Group of basic commands")]
    public class BasicCommands : ModuleBase
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
        public BasicCommands(CommandService commands, 
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
        /// This command will unlock a player
        /// </summary>
        [Command("Cancel")]
        [Summary("Unlocks player from current command")]
        public async Task CancelCommand()
        {
            var user = Context.User;
            try
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Canceling all actions for user",
                    TriggerChannel = Context.Channel.Name,
                    TriggerServer = Context.Guild.Name,
                    TriggerClass = nameof(BasicCommands),
                    TriggerFunction = nameof(CancelCommand),
                    User = ulong.Parse(user.Id.ToString())
                };

                await _lock.UnlockUser(user);
            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = "Could not cancel commands for user " + ex.Message,
                    TriggerChannel = Context.Channel.Name,
                    TriggerServer = Context.Guild.Name,
                    TriggerClass = nameof(BasicCommands),
                    TriggerFunction = nameof(CancelCommand),
                    User = ulong.Parse(user.Id.ToString())
                };
            }
        }

        /// <summary>
        /// Command that lets the player change their name
        /// </summary>
        [Command("ChangeName", RunMode = RunMode.Async)]
        [Summary("This command registers the player to the bot")]
        public async Task ChangeName()
        {
            string returnString = "";
            string action = "Changing Name";
            var user = Context.User;

            try
            {
                var lockCheck = await _lock.CheckLockout(user);

                if (lockCheck.Locked)
                {
                    returnString = user.Mention + $" is currently {lockCheck.Action}";
                    await ReplyAsync(returnString);
                    return;
                }
                else
                {
                    await _lock.LockUser(user, action);
                }

                var player = await _register.CheckRegistration(user);

                if (!player)
                {
                    returnString = user.Mention + " is not registered";
                    await ReplyAsync(returnString);

                    await _logs.ManualLog(new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        TriggerServer = Context.Guild.Name,
                        TriggerChannel = Context.Channel.Name,
                        TriggerClass = nameof(BasicCommands),
                        TriggerFunction = nameof(ChangeName),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"{user.Id} was not registered, could not change name"
                    });

                    await _lock.UnlockUser(user);
                    return;
                }
                else
                {
                    var nameRegex = ("^[a-zA-Z]{1,15}$");
                    returnString = $"{user.Mention} Please select a new name for your character";
                    await ReplyAsync(returnString);

                    var name = await _interact.NextMessageAsync(x => x.Author.Id == user.Id && x.Channel == Context.Channel);

                    var match = Regex.Match(name.Value.ToString(), nameRegex);

                    while (match.Success == false)
                    {
                        if (string.Equals(name.Value.ToString().ToLower(), "!cancel"))
                        {
                            return;
                        }
                        else if (!name.Value.ToString().StartsWith("!"))
                        {
                            returnString = $"{user.Mention} Your name was invalid.  Please use only Letters and no spaces";
                            await ReplyAsync(returnString);
                        }

                        name = await _interact.NextMessageAsync(x => x.Author.Id == user.Id && x.Channel == Context.Channel);
                        match = Regex.Match(name.Value.ToString(), nameRegex);
                    }

                    if (name.IsSuccess)
                    {
                        await _logs.ManualLog(new LogMessage
                        {
                            User = ulong.Parse(user.Id.ToString()),
                            TriggerServer = Context.Guild.Name,
                            TriggerChannel = Context.Channel.Name,
                            TriggerClass = nameof(BasicCommands),
                            TriggerFunction = nameof(ChangeName),
                            Severity = Discord.LogSeverity.Info,
                            Message = $"{user.Id} updated name to {name.Value}"
                        });
                    }

                    var registarUpdate = await _register.UpdateRegistration(user, name.Value.ToString());

                    if (!registarUpdate)
                    {
                        throw new Exception($"Update registration failed for {user.Id}");
                    }

                    await ReplyAsync($"You have updated your name, welcome back {name.Value}");

                    await _lock.UnlockUser(user);
                }
            }
            catch (Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerServer = Context.Guild.Name,
                    TriggerChannel = Context.Channel.Name,
                    TriggerClass = nameof(BasicCommands),
                    TriggerFunction = nameof(ChangeName),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"{user.Id} was unable to change name. " + ex.Message
                });

                await _lock.UnlockUser(user);
            }
        }

        /// <summary>
        /// this command is temporary, should be implimented into the register command
        /// </summary>
        // TODO: move first champ command into part of the registration process
        [Command("FirstChamp")]
        [Summary("This command gets players their first champion")]
        public async Task FirstChampCommand()
        {
            var user = Context.User;

            try
            {
                var champCount = await _champService.GetChampsCount();
                var ownedCount = await _champService.GetOwnedChampCount(user);
                var playerRegistered = await _register.CheckRegistration(user);
                var playerLocked = await _lock.CheckLockout(user);

                if (playerLocked.Locked ||
                    !playerRegistered)
                {
                    throw new Exception($"{user.Mention} is not registered, or is locked");
                }

                if (ownedCount > 0)
                {
                    throw new Exception($"{user.Mention} already owns a champion");                
                }

                var randomNumber = new Random();
                var randomChamp = randomNumber.Next(0, champCount);
                randomChamp++;

                await _champService.GiveRandomChamp(user, randomChamp);
                var newChamp = await _champService.GetChampByID(randomChamp);

                await ReplyAsync($"Congradulations you have unlocked {newChamp.ChampionName}");

            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        /// <summary>
        /// Command for setting the prefix for the bot
        /// </summary>
        /// <param name="instruction"></param>
        [Command("Prefix")]
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
                        TriggerChannel = Context.Channel.Name,
                        TriggerServer = Context.Guild.Name,
                        TriggerClass = nameof(BasicCommands),
                        TriggerFunction = nameof(PrefixCommand),
                        User = ulong.Parse(user.Id.ToString())
                    };

                    Environment.SetEnvironmentVariable("prefix", newPrefix);

                    await _logs.ManualLog(log);
                    await ReplyAsync($"your prefix was changed to {newPrefix}");
                }

            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = "Unable to set Prefix " + ex.Message,
                    TriggerChannel = Context.Channel.Name,
                    TriggerServer = Context.Guild.Name,
                    TriggerClass = nameof(BasicCommands),
                    TriggerFunction = nameof(PrefixCommand),
                    User = ulong.Parse(user.Id.ToString())
                };

            }
        }

        /// <summary>
        /// Command for new players to register themselves
        /// </summary>
        [Command("Register", RunMode = RunMode.Async)]
        [Summary("This command registers the player to the bot")]
        public async Task RegisterCommand()
        {
            string returnString = "";
            string action = "Registering";
            var nameRegex = ("^[a-zA-Z]{1,15}$");
            var user = Context.User;

            try
            {
                // Check if registered
                var registrationCheck = await _register.CheckRegistration(user);

                if (registrationCheck == true)
                {
                    returnString = user.Mention + " is already registered";
                    await ReplyAsync(returnString);

                    await _logs.ManualLog(new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        TriggerServer = Context.Guild.Name,
                        TriggerChannel = Context.Channel.Name,
                        TriggerClass = nameof(BasicCommands),
                        TriggerFunction = nameof(RegisterCommand),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"{user.Id} attempted to reregister"
                    });

                    await _lock.UnlockUser(user);
                    return;
                }

                // check if locked out
                var lockCheck = await _lock.CheckLockout(user);

                if (lockCheck.Locked)
                {
                    returnString = user.Mention + $" is currently {lockCheck.Action}";
                    await ReplyAsync(returnString);
                    return;
                }
                else
                {
                    await _lock.LockUser(user, action);
                }

                // get name for character
                returnString = $"{user.Mention} Please select a name for your character";
                await ReplyAsync(returnString);

                var name = await _interact.NextMessageAsync(x => x.Author.Id == user.Id && x.Channel == Context.Channel);

                var match = Regex.Match(name.Value.ToString(), nameRegex);

                // make sure name is a valid name
                while (match.Success == false)
                {
                    // cancel handler for exiting a command
                    if (string.Equals(name.Value.ToString().ToLower(), "!cancel"))
                    {
                        return;
                    }
                    else if (!name.Value.ToString().StartsWith("!"))
                    {
                        returnString = $"{user.Mention} Your name was invalid.  Please use only Letters and no spaces";
                        await ReplyAsync(returnString);
                    }

                    name = await _interact.NextMessageAsync(x => x.Author.Id == user.Id && x.Channel == Context.Channel);
                    match = Regex.Match(name.Value.ToString(), nameRegex);
                }

                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerServer = Context.Guild.Name,
                    TriggerChannel = Context.Channel.Name,
                    TriggerClass = nameof(BasicCommands),
                    TriggerFunction = nameof(RegisterCommand),
                    Severity = Discord.LogSeverity.Info,
                    Message = $"{user.Id} registered with name of {name.Value}"
                });

                PlayerData registration = new PlayerData
                {
                    UserID = user.Id,
                    CreationDate = DateTime.Now,
                    Name = name.Value.ToString()
                };

                registrationCheck = await _register.Registration(registration);

                if (registrationCheck == true)
                {
                    await ReplyAsync($"You have registered, welcome {name.Value}");
                }
                else
                {
                    await ReplyAsync("Registration has failed, please try again");
                }

                await _lock.UnlockUser(user);
            }
            catch (Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerServer = Context.Guild.Name,
                    TriggerChannel = Context.Channel.Name,
                    TriggerClass = nameof(BasicCommands),
                    TriggerFunction = nameof(RegisterCommand),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"{user.Id} was unable to register." + ex.Message
                });

                await _lock.UnlockUser(user);
            }
        }

        #endregion
    }
}
