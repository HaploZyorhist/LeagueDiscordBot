using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Modules.Response
{
    /// <summary>
    /// container for the data on an instance of a champion
    /// </summary>
    public class ChampionsResponse
    {
        /// <summary>
        /// discord user id
        /// </summary>
        public ulong ChampionOwner { get; set; }

        /// <summary>
        /// champion id
        /// </summary>
        public int ChampionID { get; set; }

        /// <summary>
        /// champion level
        /// </summary>
         public int ChampionLevel { get; set; }

        /// <summary>
        /// champion experience
        /// </summary>
        public int ChampionExp { get; set; }

        /// <summary>
        /// champion tier
        /// </summary>
        public int ChampionTier { get; set; }
    }
}
