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
    /// object containing contents from the Player Bag table
    /// </summary>
    public class PlayerBag
    {
        /// <summary>
        /// discord user id
        /// </summary>
        [Key]
        [Column("BagOwner", TypeName = "numeric(20, 0)")]
        public ulong BagOwner { get; set; }

        /// <summary>
        /// chompers
        /// </summary>
        [Column("Chompers", TypeName ="int")]
        public int Chompers { get; set; }

        /// <summary>
        /// Rockets
        /// </summary>
        [Column("Rockets", TypeName = "int")]
        public int Rockets { get; set; }
    }
}
