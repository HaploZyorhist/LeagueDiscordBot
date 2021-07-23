using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LeagueDiscordBot.DBTables
{
    /// <summary>
    /// class for containing table data 
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// discord user id
        /// </summary>
        [Key]
        [Column("UserId", TypeName = "numeric(20, 0)")]
        public ulong UserID { get; set; }

        /// <summary>
        /// date time for creation of player data
        /// </summary>
        [Column("CreationDate", TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// the player's name
        /// </summary>
        [Column("Name", TypeName = "varchar(15)")]
        public string Name { get; set; }
    }
}
