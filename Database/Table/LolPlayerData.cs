using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LeagueDiscordBot.Database.Table
{
    public class LolPlayerData
    {
        [Key]
        [Column("UserID", TypeName = "bigint")]
        public ulong UserID { get; set; }

        [Column("CreationDate", TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        [Column("Name", TypeName = "varchar(15)")]
        public string Name { get; set; }
    }
}
