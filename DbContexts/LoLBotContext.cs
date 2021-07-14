using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueDiscordBot.DBTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LeagueDiscordBot.DbContexts
{
    public class LoLBotContext : DbContext
    {
        public LoLBotContext()
        { }

        public virtual DbSet<LoLBotLogs> LoLBotLogs { get; set; }

        public virtual DbSet<PlayerData> PlayerData { get; set; }

        public virtual DbSet<UserLock> UserLock { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DBConnection"));
        }
    }
}
