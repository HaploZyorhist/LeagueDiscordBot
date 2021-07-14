using Discord.Commands;
using Interactivity;
using LeagueDiscordBot.DbContexts;
using LeagueDiscordBot.DBTables;
using LeagueDiscordBot.Modules;
using LeagueDiscordBot.Services;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    /// <summary>
    /// Class used to register and deregister players
    /// </summary>
    [Group("Register")]
    [Summary("This command is used to register and remove players from the game")]
    public class Register : ModuleBase
    {
        #region Fields

        private LogService _logs;
        private LockOutService _lock;
        private readonly InteractivityService _interact;

        #endregion

        #region CTOR

        public Register(LogService logs, LockOutService lockout, InteractivityService interact)
        {
            _logs = logs;
            _lock = lockout;
            _interact = interact;
        }

        #endregion

        #region Commands

        [Command(RunMode = RunMode.Async)]
        [Summary("This command registers the player to the bot")]
        public async Task RegisterCommand()
        {
            string returnString = "";
            string action = "Registering";

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

                LoLBotContext db = new LoLBotContext();
                var player = db.PlayerData.FirstOrDefault(t => t.UserID == user.Id);
                PlayerData registration = new PlayerData();

                if (player != null)
                {
                    returnString = user.Mention + " is already registered";
                    await ReplyAsync(returnString);

                    await _logs.ManualLog(new LogMessage
                        {
                            User = ulong.Parse(user.Id.ToString()),
                            TriggerServer = Context.Guild.Name,
                            TriggerChannel = Context.Channel.Name,
                            TriggerClass = nameof(Register),
                            TriggerFunction = nameof(RegisterCommand),
                            Severity = Discord.LogSeverity.Info,
                            Message = $"{user.Id} attempted to reregister"
                        });

                    await _lock.UnlockUser(user);
                    return;
                }
                else
                {
                    var nameRegex = ("^[a-zA-Z]{1,15}$");
                    returnString = $"{user.Mention} Please select a name for your character";
                    await ReplyAsync(returnString);

                    var name = await _interact.NextMessageAsync(x => x.Author.Id == user.Id && x.Channel == Context.Channel);

                    var match = Regex.Match(name.Value.ToString(), nameRegex);

                    while (match.Success == false)
                    {
                        if (string.Equals(name.Value.ToString().ToLower(), "!cancel"))
                        {
                            return;
                        } 
                        else if(!name.Value.ToString().StartsWith("!"))
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
                            TriggerClass = nameof(Register),
                            TriggerFunction = nameof(RegisterCommand),
                            Severity = Discord.LogSeverity.Info,
                            Message = $"{user.Id} registered with name of {name.Value}"
                        });

                        registration.UserID = user.Id;
                        registration.CreationDate = DateTime.Now;
                        registration.Name = name.Value.ToString();
                    }

                    db.PlayerData.Add(registration);
                    db.SaveChanges();

                    await ReplyAsync($"You have registered, welcome {name.Value}");

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
                    TriggerClass = nameof(Register),
                    TriggerFunction = nameof(RegisterCommand),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"{user.Id} was unable to register." + ex.Message
                });

                await _lock.UnlockUser(user);
            }
        }

        /// <summary>
        /// Command that lets the player change their name
        /// </summary>
        [Command("ChangeName", RunMode = RunMode.Async)]
        [Summary("This command registers the player to the bot")]
        public async Task ChangeName ()
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

                LoLBotContext db = new LoLBotContext();
                var player = db.PlayerData.FirstOrDefault(t => t.UserID == user.Id);

                if (player == null)
                {
                    returnString = user.Mention + " is not registered";
                    await ReplyAsync(returnString);

                    await _logs.ManualLog(new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        TriggerServer = Context.Guild.Name,
                        TriggerChannel = Context.Channel.Name,
                        TriggerClass = nameof(Register),
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
                            TriggerClass = nameof(Register),
                            TriggerFunction = nameof(ChangeName),
                            Severity = Discord.LogSeverity.Info,
                            Message = $"{user.Id} updated name to {name.Value}"
                        });

                        player.Name = name.Value.ToString();
                    }

                    db.PlayerData.Update(player);
                    db.SaveChanges();

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
                    TriggerClass = nameof(Register),
                    TriggerFunction = nameof(ChangeName),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"{user.Id} was unable to change name. " + ex.Message
                });

                await _lock.UnlockUser(user);
            }
        }

        #endregion
    }
}
