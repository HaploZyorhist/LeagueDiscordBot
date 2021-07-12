using Discord.Commands;
using Interactivity;
using LeagueDiscordBot.DBTables;
using LeagueDiscordBot.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    /// <summary>
    /// Class used to register and deregister players
    /// </summary>
    [Group("Register")]
    [Summary("This command is used to register and remove players from the game")]
    public class Register : ModuleBase
    {
        private LogService _logs;
        private readonly InteractivityService _interact;

        public Register(LogService logs, InteractivityService interact)
        {
            _logs = logs;
            _interact = interact;
        }

        [Command(RunMode = RunMode.Async)]
        [Summary("This command registers the player to the bot")]
        public async Task RegisterCommand()
        {
            string returnString = "";

            var user = Context.User;

            try
            {
                SqlDataReader dataReader;
                SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("DBConnection"));
                conn.Open();
                var response = new SqlCommand(@"select UserID from PlayerData where UserID = '" + user.Id + "'", conn);
                dataReader = response.ExecuteReader();

                var isRegistered = false;
                if (dataReader.HasRows)
                {
                    isRegistered = true;
                }
                
                dataReader.Close();
                response.Dispose();
                conn.Close();
                
                if (isRegistered)
                {
                    returnString = user.Mention + " is already registered";
                    await ReplyAsync(returnString);
                    return;
                }
                else
                {
                    returnString = "Please select a name for your character";
                    await ReplyAsync(returnString);

                    var name = await _interact.NextMessageAsync(x => x.Author.Id == user.Id && x.Channel == Context.Channel);

                    if (name.IsSuccess)
                    {
                        LoLPlayerBase registration = new LoLPlayerBase
                        {
                            UserID = user.Id,
                            CreationDate = DateTime.Now,
                            Name = name.Value.ToString()
                        };

                        SqlDataAdapter adapter = new SqlDataAdapter();

                        string sql = $"insert into PlayerData (UserID, CreationDate, Name) values ('{registration.UserID}', '{registration.CreationDate}', '{registration.Name}')";

                        conn.Open();
                        response = new SqlCommand(sql, conn);

                        adapter.InsertCommand = response;
                        adapter.InsertCommand.ExecuteNonQuery();

                        response.Dispose();
                        conn.Close();
                    }
                }

                await ReplyAsync("You have used register");
            }
            catch (Exception ex)
            {

            }
        }

        public async Task DeleteRegistration ()
        {

        }
    }
}
