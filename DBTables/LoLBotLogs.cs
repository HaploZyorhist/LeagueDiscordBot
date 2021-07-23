using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LeagueDiscordBot.DBTables
{
    /// <summary>
    /// data for the logs table
    /// </summary>
    public class LoLBotLogs
    {
        /// <summary>
        /// primary key for the log
        /// </summary>
        [Key]
        [Column("LogID", TypeName = "int")]
        public int LogID { get; set; }

        /// <summary>
        /// id for the user 
        /// </summary>
        [Column("UserId", TypeName = "numeric(20, 0)")]
        public ulong UserID { get; set; }

        /// <summary>
        /// date of the log
        /// </summary>
        [Column("LogDate", TypeName = "datetime")]
        public DateTime LogDate { get; set; }

        /// <summary>
        /// class that triggered log
        /// </summary>
        [Column("TriggerClass", TypeName = "varchar(25)")]
        public string TriggerClass { get; set; }

        /// <summary>
        /// function that triggered log
        /// </summary>
        [Column("TriggerFunction", TypeName = "varchar(25)")]
        public string TriggerFunction { get; set; }

        /// <summary>
        /// Server that triggered log
        /// </summary>
        [Column("TriggerServer", TypeName = "text")]
        public string TriggerServer { get; set; }

        /// <summary>
        /// Channel that triggered log
        /// </summary>
        [Column("TriggerChannel", TypeName = "text")]
        public string TriggerChannel { get; set; }

        /// <summary>
        /// severity of the log
        /// </summary>
        [Column("Severity", TypeName = "varchar(10)")]
        public string Severity { get; set; }

        /// <summary>
        /// message of the log
        /// </summary>
        [Column("LogMessage", TypeName = "text")]
        public string LogMessage { get; set; }
    }
}
