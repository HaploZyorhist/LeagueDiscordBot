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
    /// service for handling data from champion base
    /// </summary>
    public class ChampionService
    {
        #region Fields

        private readonly LogService _logs;
        private readonly LoLBotContext _db;

        #endregion

        #region CTOR

        /// <summary>
        /// CTOR class for log service
        /// </summary>
        /// <param name="discord"></param>
        /// <param name="commands"></param>
        public ChampionService(LogService logs, LoLBotContext db)
        {
            _logs = logs;
            _db = db;
        }

        #endregion

        #region Champion Service

        /// <summary>
        /// Generats a count of champions 
        /// </summary>
        public async Task<int> GetChampsCount()
        {
            try
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Generating count of champions",
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetChampsCount)
                };
                var champCount = await _db.ChampionBase.CountAsync();

                return champCount;

            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetChampsCount)
                };
                return 0;
            }
        }

        /// <summary>
        /// Generats a list of champions by name
        /// </summary>
        /// <param name="championName"></param>
        public async Task<List<ChampionBaseResponse>> GetChampsByName (string championName)
        {
            try
            {
                var champList = await _db.ChampionBase.AsAsyncEnumerable().Where(c => c.ChampionName == championName).ToListAsync();

                List<ChampionBaseResponse> champResponseList = new List<ChampionBaseResponse>();

                foreach (var champ in champList)
                {
                    var log = new LogMessage
                    {
                        Severity = Discord.LogSeverity.Info,
                        Message = "Getting champion by name",
                        TriggerClass = nameof(ChampionService),
                        TriggerFunction = nameof(GetChampsByName)
                    };
                    ChampionBaseResponse champResponse = new ChampionBaseResponse
                    {
                        ChampionID = champ.ChampionID,
                        ChampionName = champ.ChampionName,
                        Series = champ.Series
                    };

                    champResponseList.Add(champResponse);
                }

                return champResponseList;
              
            }
            catch(Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetChampsByName)
                };
                return null;
            }
        }

        /// <summary>
        /// Generates a list of champions by series
        /// </summary>
        /// <param name="seriesName"></param>
        public async Task<List<ChampionBaseResponse>> GetChampsBySeries(string seriesName)
        {
            try
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Getting champion by series",
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetChampsBySeries)
                };

                var champList = await _db.ChampionBase.AsAsyncEnumerable().Where(c => c.Series == seriesName).ToListAsync();

                List<ChampionBaseResponse> champResponseList = new List<ChampionBaseResponse>();

                foreach (var champ in champList)
                {

                    ChampionBaseResponse champResponse = new ChampionBaseResponse
                    {
                        ChampionID = champ.ChampionID,
                        ChampionName = champ.ChampionName,
                        Series = champ.Series
                    };

                    champResponseList.Add(champResponse);
                }

                return champResponseList;
            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetChampsBySeries)
                };
                return null;
            }

        }

        /// <summary>
        /// Gets a champion by id
        /// </summary>
        /// <param name="championID"></param>
        public async Task<ChampionBaseResponse> GetChampByID(int championID)
        {
            try
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Getting champion by ID",
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetChampByID)
                };
                var champ = await _db.ChampionBase.FirstOrDefaultAsync(c => c.ChampionID == championID);

                ChampionBaseResponse champResponse = new ChampionBaseResponse
                {
                    ChampionID = champ.ChampionID,
                    ChampionName = champ.ChampionName,
                    Series = champ.Series
                };

                return champResponse;
            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetChampByID)
                };
            }
            return null;
        }

        /// <summary>
        /// Generates a count of champions that the player owns
        /// </summary>
        /// <param name="user"></param>
        public async Task<int> GetOwnedChampCount (Discord.IUser user)
        {
            try
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Getting count of owned champs",
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetOwnedChampCount)
                };
                var ownedCount = await _db.Champions.CountAsync(c => c.ChampionOwner == ulong.Parse(user.Id.ToString()));

                return ownedCount;
            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetOwnedChampCount)
                };
                return 0;
            }
        }

        /// <summary>
        /// Gives a random champion to the player
        /// </summary>
        /// <param name="user"></param>
        public async Task GiveRandomChamp (Discord.IUser user)
        {
            try
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Giving a random champ to player",
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GiveRandomChamp)
                };
                Random randomNum = new Random();

                var champID = randomNum.Next(1, 4);
                Champions newChamp = new Champions
                {
                    ChampionOwner = ulong.Parse(user.Id.ToString()),
                    ChampionID = champID,
                    ChampionLevel = 1,
                    ChampionExp = 0,
                    ChampionTier = 1
                };

                await _db.Champions.AddAsync(newChamp);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GiveRandomChamp)
                };
            }
        }

        /// <summary>
        /// Gets a list of champions that the player owns
        /// </summary>
        /// <param name="user"></param>
        public async Task<List<ChampionsResponse>> GetMyChamps(Discord.IUser user)
        {
            try
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Generating a list of player owned champs",
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetMyChamps)
                };

                List<ChampionsResponse> myChamps = new List<ChampionsResponse>();

                var champList = await _db.Champions.AsAsyncEnumerable().Where(c => c.ChampionOwner == ulong.Parse(user.Id.ToString())).ToListAsync();

                foreach (var champ in champList)
                {
                    ChampionsResponse champion = new ChampionsResponse
                    {
                        ChampionID = champ.ChampionID,
                        ChampionOwner = champ.ChampionOwner,
                        ChampionTier = champ.ChampionTier,
                        ChampionLevel = champ.ChampionLevel,
                        ChampionExp = champ.ChampionExp
                    };

                    myChamps.Add(champion);
                }

                return myChamps;
            }
            catch(Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetMyChamps)
                };
                return null;
            }
        }

        /// <summary>
        /// Generates a list of tiers for champions
        /// </summary>
        public async Task<Dictionary<int, string>> GetTierList()
        {
            try
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Info,
                    Message = "Generating a list of champion tiers",
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetTierList)
                };

                var getTier = await _db.ChampionTier.AsAsyncEnumerable().ToListAsync();
                Dictionary<int, string> tierList = new Dictionary<int, string>();

                foreach(var tier in getTier)
                {
                    tierList.Add(tier.TierID, tier.TierName);
                };

                return tierList;
            }
            catch (Exception ex)
            {
                var log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                    TriggerClass = nameof(ChampionService),
                    TriggerFunction = nameof(GetTierList)
                };

                return null;
            }
        }

        #endregion
    }
}
