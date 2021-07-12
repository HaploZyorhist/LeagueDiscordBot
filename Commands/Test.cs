using Autofac.Features.OwnedInstances;
using Discord.Commands;
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
            await ReplyAsync("You have used Test");
        }

        [Command("NotTest")]
        [Summary("This is not a test class")]
        public async Task NotTestCommand()
        {
            await ReplyAsync("you have used the not test command");
        }
    }
}
