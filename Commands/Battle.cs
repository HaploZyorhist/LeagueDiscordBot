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

        private readonly LogService _logs;
        private readonly LockOutService _lock;
        private readonly RegistrationService _register;
        private readonly InteractivityService _interact;
        private readonly BattleService _battle;

        #endregion

        #region CTOR

        public Battle(LogService logs, 
                      LockOutService lockOut, 
                      RegistrationService register, 
                      InteractivityService interact,
                      BattleService battle)
        {
            _logs = logs;
            _lock = lockOut;
            _register = register;
            _interact = interact;
            _battle = battle;
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

                bool acceptVar = false;
                DateTime challengeTime = DateTime.Now;

                InteractivityResult<Discord.WebSocket.SocketMessage> challengeResult = null;

                while (acceptVar == false && challengeTime.AddSeconds(30) > DateTime.Now)
                {
                    challengeResult = await _interact.NextMessageAsync(x => (x.Author.Id == challengedPlayer.Id || x.Author.Id == user.Id) && x.Channel == Context.Channel);
                    
                    if (challengeResult.Value == null || challengeTime.AddSeconds(30) <= DateTime.Now)
                    {
                        throw new Exception("The challenge has timed out");
                    }

                    if ((challengeResult.Value.Author.Id == user.Id && challengeResult.Value.ToString().ToLower() == "!cancel") ||
                       (challengeResult.Value.Author.Id == challengedPlayer.Id && (challengeResult.Value.ToString().ToLower() == "deny" ||
                                                                                   challengeResult.Value.ToString().ToLower() == "!cancel")))
                    {
                        throw new Exception("The battle was canceled");
                    }

                    if (challengeResult.Value.Author.Id == challengedPlayer.Id && (challengeResult.Value.ToString().ToLower() == "accept"))
                    {
                        acceptVar = true;
                        var winner = await _battle.Battle(user, challengedPlayer, Context.Channel);

                        if (winner == null)
                        {
                            throw new Exception("The battle was canceled, no one has won");
                        }

                        if (winner == user)
                        {
                            // TODO: Add rewards for winner
                            await ReplyAsync($"Congrats {user.Mention} you have won the fight");
                        }

                        if (winner == challengedPlayer)
                        {
                            // TODO: Add rewards for winner
                            await ReplyAsync($"Congrats {challengedPlayer.Mention} you have won the fight");
                        }

                        await _lock.UnlockUser(user);
                        await _lock.UnlockUser(challengedPlayer);
                    }
                }

                return;
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

                await ReplyAsync(ex.Message);

                await _lock.UnlockUser(user);

                if (challengedPlayer != null)
                {
                    await _lock.UnlockUser(challengedPlayer);
                }

                return;
            }
        }

        #endregion

        #region PVE Battles

        #endregion
    }
}
