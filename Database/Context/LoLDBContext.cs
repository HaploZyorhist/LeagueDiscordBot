using LeagueDiscordBot.Database.Table;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueDiscordBot.Database.Context
{
    public class LoLDBContext : DbContext
    {
        public LoLDBContext(DbContextOptions<LoLDBContext> options) :base (options)
        {
        }

        public virtual DbSet<LolPlayerData> LoLPlayerData { get; set; }
    }
}
