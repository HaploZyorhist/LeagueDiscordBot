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
    /// object for containing data from Champion Base table
    /// </summary>
    public class ChampionBase
    {
        /// <summary>
        /// primary key for the log
        /// </summary>
        [Key]
        [Column("ChampionID", TypeName = "int")]
        public int ChampionID { get; set; }

        /// <summary>
        /// name of the champion
        /// </summary>
        [Column("ChampionName", TypeName = "varchar(50)")]
        public string ChampionName { get; set; }

        /// <summary>
        /// series that the champion originates from
        /// </summary>
        [Column("Series", TypeName = "varchar(25)")]
        public string Series { get; set; }
    }
}
