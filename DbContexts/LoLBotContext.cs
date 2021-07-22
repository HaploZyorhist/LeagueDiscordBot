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
        public LoLBotContext(DbContextOptions<LoLBotContext> options) : base(options)
        { }

        public virtual DbSet<ChampionBase> ChampionBase { get; set; }

        public virtual DbSet<Champions> Champions { get; set; }

        public virtual DbSet<ChampionTier> ChampionTier { get; set; }

        public virtual DbSet<LoLBotLogs> LoLBotLogs { get; set; }

        public virtual DbSet<PlayerData> PlayerData { get; set; }

        public virtual DbSet<UserLock> UserLock { get; set; }
    }
}
