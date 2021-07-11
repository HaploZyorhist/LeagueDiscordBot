using Autofac.Features.OwnedInstances;
using Discord.Commands;
using LeagueDiscordBot.Database.Context;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    [Group("Testy")]
    [Summary("Testing the main group")]
    public class Test : ModuleBase
    {

        [Command("Test")]
        [Summary("The actual test")]
        public async Task TestCommand()
        {
            SqlDataReader dataReader;
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("DBConnection"));
            conn.Open();
            var response = new SqlCommand(@"select UserID from LoLPlayerData", conn);
            dataReader = response.ExecuteReader();

            while (dataReader.Read())
            {
                var output = dataReader.GetValue(0);
            }

            dataReader.Close();
            response.Dispose();
            conn.Close();
            await ReplyAsync("You have used Test");
        }
    }
}
