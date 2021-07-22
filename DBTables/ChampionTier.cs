using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.DBTables
{
    /// <summary>
    /// object containing data from Champion Tier table
    /// </summary>
    public class ChampionTier
    {
        /// <summary>
        /// primary key for the table
        /// </summary>
        [Key]
        [Column("TierID", TypeName = "int")]
        public int TierID { get; set; }

        /// <summary>
        /// name of the tier
        /// </summary>
        [Column("TierName", TypeName = "varchar(12)")]
        public string TierName { get; set; }
    }
}
