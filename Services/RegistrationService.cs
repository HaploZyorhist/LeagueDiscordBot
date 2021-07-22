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
    /// Service for managing and checking registrations
    /// </summary>
    public class RegistrationService
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
        public RegistrationService(DiscordSocketClient discord, LogService logs, LoLBotContext db)
        {
            _discord = discord;
            _logs = logs;
            _db = db;

        }

        #endregion

        #region Registration Service

        /// <summary>
        /// method for checking the registration of a user
        /// </summary>
        /// <param name="user"></param>
        public async Task<bool> CheckRegistration (Discord.IUser user)
        {
            try
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(RegistrationService),
                    TriggerFunction = nameof(CheckRegistration),
                    Severity = Discord.LogSeverity.Info,
                    Message = $"Checking registration for {user.Id}"
                });

                var player = await _db.PlayerData.FirstOrDefaultAsync(t => t.UserID == user.Id);

                if (player != null)
                {
                    await _logs.ManualLog(new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        TriggerClass = nameof(RegistrationService),
                        TriggerFunction = nameof(CheckRegistration),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"{user.Id} registration found"
                    });
                    return true;
                }
                else
                {
                    await _logs.ManualLog(new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        TriggerClass = nameof(RegistrationService),
                        TriggerFunction = nameof(CheckRegistration),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"{user.Id} registration not found"
                    });
                    return false;
                }
            }
            catch (Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.Id.ToString()),
                    TriggerClass = nameof(RegistrationService),
                    TriggerFunction = nameof(CheckRegistration),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"{user.Id} registration check failed " + ex.Message
                });

                return true;
            }
        }

        /// <summary>
        /// method for registering a new user
        /// </summary>
        /// <param name="user"></param>
        public async Task<bool> Registration(PlayerData user)
        {
            try
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.UserID.ToString()),
                    TriggerClass = nameof(RegistrationService),
                    TriggerFunction = nameof(Registration),
                    Severity = Discord.LogSeverity.Info,
                    Message = $"Attempting to register {user.UserID}"
                });

                await _db.PlayerData.AddAsync(user);
                await _db.SaveChangesAsync();

                return true;
            }
            catch(Exception ex)
            {
                await _logs.ManualLog(new LogMessage
                {
                    User = ulong.Parse(user.UserID.ToString()),
                    TriggerClass = nameof(RegistrationService),
                    TriggerFunction = nameof(Registration),
                    Severity = Discord.LogSeverity.Error,
                    Message = $"Registration failed for {user.UserID} " + ex.Message
                });

                return false;
            }
        }
        // TODO: finish update registration method
        public async Task<bool> UpdateRegistration(Discord.IUser user, string newName)
        {
            return false;
        }

        // TODO: finish ban registration method
        public async Task<bool> BanRegistration(Discord.IUser user)
        {
            return false;
        }

        #endregion
    }
}
