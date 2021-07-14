using Discord.WebSocket;
using LeagueDiscordBot.DbContexts;
using LeagueDiscordBot.DBTables;
using LeagueDiscordBot.Modules;
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
        private LogService _logs;

        #endregion

        #region CTOR

        /// <summary>
        /// CTOR class for log service
        /// </summary>
        /// <param name="discord"></param>
        /// <param name="commands"></param>
        public LockOutService(DiscordSocketClient discord, LogService logs)
        {
            _discord = discord;
            _logs = logs;
        }

        #endregion

        #region Locking Service

        /// <summary>
        /// method for checking if user is currently locked out
        /// </summary>
        /// <param name="user">the user identity who is being checked</param>
        /// <returns>true if user is locked out, false if they are not</returns>
        public async Task<bool> CheckLockout(Discord.IUser user)
        {
            try
            {
                LoLBotContext db = new LoLBotContext();

                var lockout = db.UserLock.FirstOrDefault(u => u.UserID == user.Id);

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
                    db.UserLock.Add(new UserLock
                    {
                        UserID = ulong.Parse(user.Id.ToString()),
                        LockOut = false,
                        DateTime = DateTime.Now
                    });

                    db.SaveChanges();

                    await _logs.ManualLog(new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        TriggerClass = nameof(LockOutService),
                        TriggerFunction = nameof(CheckLockout),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"{user.Id} was not in lockout table, added to table."
                    });

                    return false;
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

                    return true;
                }

                return false;
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

                return false;
            }
        }

        /// <summary>
        /// method for locking user
        /// </summary>
        /// <param name="user">the user identity who is being locked</param>
        public async Task<bool> LockUser(Discord.IUser user)
        {
            try
            {
                LoLBotContext db = new LoLBotContext();

                var entity = db.UserLock.FirstOrDefault(u => u.UserID == user.Id);

                entity.DateTime = DateTime.Now;
                entity.LockOut = true;

                db.UserLock.Update(entity);
                db.SaveChanges();

                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(LockOutService),
                    TriggerFunction = nameof(LockUser),
                    Severity = Discord.LogSeverity.Info,
                    Message = $"{user.Id} was locked."
                });

                return false;
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
                LoLBotContext db = new LoLBotContext();

                var entity = db.UserLock.FirstOrDefault(u => u.UserID == user.Id);

                entity.DateTime = DateTime.Now;
                entity.LockOut = false;

                db.UserLock.Update(entity);
                db.SaveChanges();

                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(LockOutService),
                    TriggerFunction = nameof(UnlockUser),
                    Severity = Discord.LogSeverity.Info,
                    Message = $"{user.Id} was unlocked."
                });

                return false;
            }
            catch (Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
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
