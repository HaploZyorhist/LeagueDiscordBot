using Discord.WebSocket;
using LeagueDiscordBot.DbContexts;
using LeagueDiscordBot.DBTables;
using LeagueDiscordBot.Modules;
using LeagueDiscordBot.Modules.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Services
{
    /// <summary>
    /// service for managing user lockouts
    /// </summary>
    public class LockOutService
    {
        #region Fields

        private readonly DiscordSocketClient _discord;
        private readonly LogService _logs;
        private readonly LoLBotContext _db;

        #endregion

        #region CTOR

        /// <summary>
        /// CTOR class for log service
        /// </summary>
        /// <param name="discord"></param>
        /// <param name="commands"></param>
        public LockOutService(DiscordSocketClient discord, LogService logs, LoLBotContext db)
        {
            _discord = discord;
            _logs = logs;
            _db = db;
        }

        #endregion

        #region Locking Service

        /// <summary>
        /// method for checking if user is currently locked out
        /// </summary>
        /// <param name="user">the user identity who is being checked</param>
        /// <returns>true if user is locked out, false if they are not</returns>
        public async Task<LockOutResponse> CheckLockout(Discord.IUser user)
        {
            LockOutResponse response = new LockOutResponse
            {
                Locked = false,
                Action = ""
            };
            try
            {
                var lockout = _db.UserLock.FirstOrDefault(u => u.UserID == user.Id);

                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(LockOutService),
                    TriggerFunction = nameof(CheckLockout),
                    Severity = Discord.LogSeverity.Info,
                    Message = $"{user.Id} was checked for lockout."
                });

                if (lockout == null)
                {
                    await _db.UserLock.AddAsync(new UserLock
                    {
                        UserID = ulong.Parse(user.Id.ToString()),
                        LockOut = false,
                        DateTime = DateTime.Now
                    });

                    await _db.SaveChangesAsync();

                    await _logs.ManualLog(new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        TriggerClass = nameof(LockOutService),
                        TriggerFunction = nameof(CheckLockout),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"{user.Id} was not in lockout table, added to table."
                    });

                    return response;
                }
                else if (lockout.LockOut)
                {
                    await _logs.ManualLog(new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        TriggerClass = nameof(LockOutService),
                        TriggerFunction = nameof(CheckLockout),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"{user.Id} was currently locked."
                    });

                    response.Locked = lockout.LockOut;
                    response.Action = lockout.Action;

                    return response;
                }

                return response;
            }
            catch (Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(LockOutService),
                    TriggerFunction = nameof(CheckLockout),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"Lockout Check for {user.Id} failed " + ex.Message
                });

                return response;
            }
        }

        /// <summary>
        /// method for locking user
        /// </summary>
        /// <param name="user">the user identity who is being locked</param>
        public async Task<bool> LockUser(Discord.IUser user, string action)
        {
            try
            {
                var entity = _db.UserLock.FirstOrDefault(u => u.UserID == user.Id);

                entity.DateTime = DateTime.Now;
                entity.LockOut = true;
                entity.Action = action;

                await _db.SaveChangesAsync();

                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(LockOutService),
                    TriggerFunction = nameof(LockUser),
                    Severity = Discord.LogSeverity.Info,
                    Message = $"{user.Id} was locked."
                });

                return true;
            }
            catch (Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(LockOutService),
                    TriggerFunction = nameof(LockUser),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"{user.Id} Locking failed " + ex.Message
                });

                return false;
            }
        }

        /// <summary>
        /// method for unlocking user
        /// </summary>
        /// <param name="user">the user identity who is being unlocked</param>
        public async Task<bool> UnlockUser(Discord.IUser user)
        {
            try
            {
                var entity = _db.UserLock.FirstOrDefault(u => u.UserID == user.Id);

                entity.DateTime = DateTime.Now;
                entity.LockOut = false;
                entity.Action = "";

                await _db.SaveChangesAsync();

                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(LockOutService),
                    TriggerFunction = nameof(UnlockUser),
                    Severity = Discord.LogSeverity.Info,
                    Message = $"{user.Id} was unlocked."
                });

                return true;
            }
            catch (Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString() ?? "2005"),
                    TriggerClass = nameof(LockOutService),
                    TriggerFunction = nameof(UnlockUser),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"{user.Id} Unlocking failed " + ex.Message
                });

                return false;
            }
        }

        #endregion
    }
}
