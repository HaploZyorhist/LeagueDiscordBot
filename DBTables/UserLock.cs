#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueDiscordBot.DBTables
{
    /// <summary>
    /// class for containing UserLock table data
    /// </summary>
    public class UserLock
    {
        /// <summary>
        /// discord user id
        /// </summary>
        [Key]
        [Column("UserID", TypeName = "numeric(20, 0)")]
        public ulong UserID { get; set; }

        /// <summary>
        /// date time for creation of player data
        /// </summary>
        [Column("DateTime", TypeName = "datetime")]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// lock out of user
        /// </summary>
        [Column("LockOut", TypeName = "bit")]
        public bool LockOut { get; set; }

        /// <summary>
        /// action that i locking the player
        /// </summary>
        [Column("Action", TypeName = "varchar(20)")]
        public string? Action { get; set; }
    }
}
