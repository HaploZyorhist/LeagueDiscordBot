using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Modules.Response
{
    /// <summary>
    /// Container for getting champion tier data
    /// </summary>
    public class TierResponse
    {
        /// <summary>
        /// champion tier ID
        /// </summary>
        public int TierID { get; set; }

        /// <summary>
        /// champion tier Name
        /// </summary>
        public string TierName { get; set; }
    }
}
