using Discord.Commands;
using LeagueDiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    public class Ping : ModuleBase
    {
        [RequireUserPermission(Discord.GuildPermission.AddReactions, Group = "Permission")]
        [Command("ping")]
        public async Task PingCommand()
        {
            var returnString = new StringBuilder();

            var user = Context.User;

            returnString.AppendLine($"You are {user.Mention}");

            await ReplyAsync(returnString.ToString());

            bool getRepeats = false;
            int Repeats = 0;

            //LogMessage log = new LogMessage();

            //try
            //{
            //    //client.

            //    if (!string.IsNullOrEmpty(instruction))
            //    {
            //        getRepeats = int.TryParse(instruction, out Repeats);
            //    }
            //    else if (string.IsNullOrEmpty(instruction))
            //    {
            //        getRepeats = true;
            //        Repeats = 1;
            //    }
            //    if (getRepeats)
            //    {
            //        log = new LogMessage
            //        {
            //            User = message.Author.ToString(),
            //            Severity = Discord.LogSeverity.Info,
            //            Message = $"Ping was pressed {Repeats} times",
            //            SourceClass = nameof(CommandHandlerService),
            //            SourceMethod = nameof(MessageReceived)
            //        };

            //        string output = "";

            //        for (var i = 0; i < Repeats; i++)
            //        {
            //            output += $"{message.Author.Mention}" + " Pong!" + "\n\r";
            //        }

            //        await _logs.ManualLog(log);
            //        await message.Channel.SendMessageAsync(output);
            //    }
            //    else if (instruction.StartsWith("<@") && instruction.EndsWith(">"))
            //    {
            //        var id = instruction.Replace("<@", "").Replace(">", "");
            //        if (id.StartsWith("!"))
            //        {
            //            id = id.Replace("!", "");
            //        }

            //        log = new LogMessage
            //        {
            //            User = message.Author.ToString(),
            //            Severity = Discord.LogSeverity.Info,
            //            Message = $"<@{id}> was Pinged!",
            //            SourceClass = nameof(CommandHandlerService),
            //            SourceMethod = nameof(MessageReceived)
            //        };

            //        await _logs.ManualLog(log);
            //        await message.Channel.SendMessageAsync($"<@{id}>" + " Pong!");
            //    }
            //    else
            //    {
            //        throw new Exception($"{nameof(Ping)} was called incorrectly");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log = new LogMessage
            //    {
            //        Severity = Discord.LogSeverity.Error,
            //        Message = ex.Message,
            //        SourceClass = nameof(CommandHandlerService),
            //        SourceMethod = nameof(Ping)
            //    };

            //    await _logs.ManualLog(log);
            //    await message.Channel.SendMessageAsync($"{nameof(Ping)} failed");
            //}
        }
    }
}
