using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueDiscordBot.DBTables
{
    /// <summary>
    /// class for containing table data 
    /// </summary>
    public class LoLPlayerBase
    {
        /// <summary>
        /// discord user id
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// date time for creation of player data
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// the player's name
        /// </summary>
        public string Name { get; set; }
    }
}
