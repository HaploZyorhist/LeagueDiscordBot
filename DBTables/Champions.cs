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
    /// container for data from the champions table
    /// </summary>
    public class Champions
    {
        /// <summary>
        /// discord user id
        /// </summary>
        [Key]
        [Column("ChampionInstance", TypeName = "int")]
        public int ChampionInstance { get; set; }
        /// <summary>
        /// discord user id
        /// </summary>
        [Column("ChampionOwner", TypeName = "numeric(20, 0)")]
        public ulong ChampionOwner { get; set; }

        /// <summary>
        /// champion id
        /// </summary>
        [Column("ChampionID", TypeName = "int")]
        public int ChampionID { get; set; }

        /// <summary>
        /// champion level
        /// </summary>
        [Column("ChampionLevel", TypeName = "int")]
        public int ChampionLevel { get; set; }

        /// <summary>
        /// champion experience
        /// </summary>
        [Column("ChampionExp", TypeName = "int")]
        public int ChampionExp { get; set; }

        /// <summary>
        /// champion tier
        /// </summary>
        [Column("ChampionTier", TypeName = "int")]
        public int ChampionTier { get; set; }
    }
}
