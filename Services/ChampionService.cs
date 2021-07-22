using Discord.WebSocket;
using LeagueDiscordBot.DbContexts;
using LeagueDiscordBot.DBTables;
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
        // TODO: add logging to champion base service

        public async Task<int> GetChampsCount()
        {
            try
            {
                var champCount = await _db.ChampionBase.CountAsync();

                return champCount;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<ChampionBaseResponse>> GetChampsByName (string championName)
        {
            try
            {
                var champList = await _db.ChampionBase.AsAsyncEnumerable().Where(c => c.ChampionName == championName).ToListAsync();

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
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ChampionBaseResponse>> GetChampsBySeries(string seriesName)
        {
            try
            {
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
                return null;
            }

        }

        public async Task<ChampionBaseResponse> GetChampByID(int championID)
        {
            try
            {
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

            }
            return null;
        }

        public async Task<int> GetOwnedChampCount (Discord.IUser user)
        {
            try
            {
                var ownedCount = await _db.Champions.CountAsync(c => c.ChampionOwner == ulong.Parse(user.Id.ToString()));

                return ownedCount;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task GiveRandomChamp (Discord.IUser user, int champID)
        {
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

        public async Task<List<ChampionsResponse>> GetMyChamps(Discord.IUser user)
        {
            try
            {
                List<ChampionsResponse> myChamps = new List<ChampionsResponse>();

                var champList = await _db.Champions.AsAsyncEnumerable().Where(c => c.ChampionOwner == ulong.Parse(user.Id.ToString())).ToListAsync();
                var ironIV = await GetTierID("Iron IV");

                foreach (var champ in champList)
                {
                    ChampionsResponse champion = new ChampionsResponse
                    {
                        ChampionID = champ.ChampionID,
                        ChampionOwner = champ.ChampionOwner,
                        ChampionTier = ironIV.TierID,
                        ChampionLevel = champ.ChampionLevel,
                        ChampionExp = champ.ChampionExp
                    };

                    myChamps.Add(champion);
                }

                return myChamps;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<TierResponse> GetTierName(int championTier)
        {
            try
            {
                var getTier = await _db.ChampionTier.FirstOrDefaultAsync(t => t.TierID == championTier);

                var response = new TierResponse
                {
                    TierID = getTier.TierID,
                    TierName = getTier.TierName
                };

                return response;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<TierResponse> GetTierID(string championTier)
        {
            try
            {
                var getTier = await _db.ChampionTier.FirstOrDefaultAsync(t => t.TierName == championTier);

                var response = new TierResponse
                {
                    TierID = getTier.TierID,
                    TierName = getTier.TierName
                };

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }
}
