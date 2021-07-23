using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Modules.Response
{
    /// <summary>
    /// object for containing information regarding champion base data
    /// </summary>
    public class ChampionBaseResponse
    {
        /// <summary>
        /// primary key for the log
        /// </summary>
        public int ChampionID { get; set; }

        /// <summary>
        /// name of the champion
        /// </summary>
        public string ChampionName { get; set; }

        /// <summary>
        /// series that the champion originates from
        /// </summary>
        public string Series { get; set; }
    }
}
