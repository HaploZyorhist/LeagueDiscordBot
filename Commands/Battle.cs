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
    /// Command group for battling
    /// </summary>
    [Group("")]
    [Summary("Commands for handling battles")]
    public class Battle : ModuleBase
    {
        #region Fields

        private readonly CommandService _commands;
        private readonly LogService _logs;
        private readonly LockOutService _lock;
        private readonly RegistrationService _register;
        private readonly InteractivityService _interact;

        #endregion

        #region CTOR

        public Battle(CommandService commands, LogService logs, LockOutService lockOut, RegistrationService register, InteractivityService interact)
        {
            _logs = logs;
            _lock = lockOut;
            _register = register;
            _interact = interact;
        }

        #endregion

        #region PVP Battles

        /// <summary>
        /// Command for challenging another player
        /// </summary>
        /// <param name="instruction"></param>
        [Command("Challenge", RunMode = RunMode.Async)]
        public async Task ChallengeCommand ([Remainder] string instruction)
        {
            var user = Context.User;
            string returnString = "";
            string action = "Challenging";
            Discord.IUser challengedPlayer = null;

            try
            {
                // Check if player is registered
                var registrationCheck = await _register.CheckRegistration(user);

                if (registrationCheck != true)
                {
                    returnString = user.Mention + " is not registered";
                    await ReplyAsync(returnString);

                    await _lock.UnlockUser(user);
                    return;
                }

                // Check if player is locked
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

                // Parse out person being challenged
                if (string.IsNullOrEmpty(instruction))
                {
                    throw new Exception($"{user.Id} Challenge failed, no user was challenged");
                }
                else if (!instruction.StartsWith("<@") || !instruction.EndsWith(">"))
                {
                    throw new Exception($"{user.Id} Challenge failed, no user was challenged");
                }
                else if (instruction.StartsWith("<@") && instruction.EndsWith(">"))
                {
                    var id = instruction.Replace("<@", "").Replace(">", "");
                    if (id.StartsWith("!"))
                    {
                        id = id.Replace("!", "");
                    }
                    
                    challengedPlayer = await Context.Client.GetUserAsync(ulong.Parse(id));
                }

                // Check if challenged player is registered
                registrationCheck = await _register.CheckRegistration(challengedPlayer);

                if (registrationCheck != true)
                {
                    returnString = challengedPlayer.Mention + " is not registered";
                    await ReplyAsync(returnString);

                    await _lock.UnlockUser(user);
                    return;
                }

                // Check if challenged player is locked
                lockCheck = await _lock.CheckLockout(challengedPlayer);

                if (lockCheck.Locked)
                {
                    returnString = $"{challengedPlayer.Mention} is currently {lockCheck.Action}";
                    await ReplyAsync(returnString);
                    await _lock.UnlockUser(user);
                    return;
                }
                else
                {
                    await _lock.LockUser(challengedPlayer, action);
                }

                // TODO: add cancel handler to message waiting for BOTH players
                returnString = $"{user.Mention} has challenged {challengedPlayer.Mention} to a battle, {challengedPlayer.Mention} you have 30 seconds to accept! (to accept type Accept)";
                await ReplyAsync(returnString);

                var challengeResponse = await _interact.NextMessageAsync(x => x.Author.Id == challengedPlayer.Id && x.Channel == Context.Channel);

                if (challengeResponse.Value == null)
                {
                    returnString = $"{challengedPlayer.Mention} failed to respond";
                    await ReplyAsync(returnString);
                    throw new Exception($"{challengedPlayer.Mention} failed to respond");
                }

                if (challengeResponse.Value.ToString().ToLower() == "accept")
                {
                    var randomPlayer = new Random();
                    var player = randomPlayer.Next(1, 3);

                    if (player == 1)
                    {
                        returnString = $"Congratulations {challengedPlayer.Mention} you have won";
                    }
                    else if (player == 2)
                    {
                        returnString = $"Congratulations {user.Mention} you have won";
                    }

                    await ReplyAsync(returnString);

                    await _lock.UnlockUser(user);
                    await _lock.UnlockUser(challengedPlayer);
                }
                else
                {
                    returnString = $"{user.Mention} your opponent was too cowerdly to play, you have executed them for turning their back";
                    await ReplyAsync(returnString);
                    await _lock.UnlockUser(user);
                    await _lock.UnlockUser(challengedPlayer);
                }

            }
            catch(Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerServer = Context.Guild.Name,
                    TriggerChannel = Context.Channel.Name,
                    TriggerClass = nameof(Battle),
                    TriggerFunction = nameof(ChallengeCommand),
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message
                });

                await ReplyAsync($"{user.Mention}, your challenge failed");

                await _lock.UnlockUser(user);

                if (challengedPlayer != null)
                {
                    await _lock.UnlockUser(challengedPlayer);
                }
            }
        }

        #endregion

        #region PVE Battles

        #endregion
    }
}
