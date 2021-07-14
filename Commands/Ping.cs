#nullable enable
using Discord.Commands;
using LeagueDiscordBot.Modules;
using LeagueDiscordBot.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Commands
{
    [Group("Ping")]
    public class Ping : ModuleBase
    {
        private readonly LogService _logs;
        private LockOutService _lock;

        public Ping (LogService logs, LockOutService lockout)
        {
            _logs = logs;
            _lock = lockout;
        }

        [RequireUserPermission(Discord.GuildPermission.Administrator, Group = "Permission")]
        [Command]
        [Summary("Ping command for testing\n" +
                 "Ping {X} will ping X times\n" +
                 "Ping {@user} will ping that user")]
        public async Task PingCommand([Remainder] string? instruction = "")
        {
            var returnString = new StringBuilder();

            var user = Context.User;

            bool getRepeats = false;
            int Repeats = 0;

            LogMessage log = new LogMessage();
            
            try
            {
                if (!string.IsNullOrEmpty(instruction))
                {
                    getRepeats = int.TryParse(instruction, out Repeats);
                }
                else if (string.IsNullOrEmpty(instruction))
                {
                    getRepeats = true;
                    Repeats = 1;
                }
                if (getRepeats)
                {
                    log = new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"Ping was pressed {Repeats} times",
                    };

                    for (var i = 0; i < Repeats; i++)
                    {
                        returnString.AppendLine($"{user.Mention}" + " Pong!");
                    }

                    await _logs.ManualLog(log);
                }
                else if (instruction.StartsWith("<@") && instruction.EndsWith(">"))
                {
                    var id = instruction.Replace("<@", "").Replace(">", "");
                    if (id.StartsWith("!"))
                    {
                        id = id.Replace("!", "");
                    }

                    log = new LogMessage
                    {
                        User = ulong.Parse(user.Id.ToString()),
                        Severity = Discord.LogSeverity.Info,
                        Message = $"<@{id}> was Pinged!",
                    };

                    await _logs.ManualLog(log);
                    returnString.AppendLine($"<@{id}> Pong!");
                }
                else
                {
                    throw new Exception($"{nameof(Ping)} was called incorrectly");
                }

                var lockCheck = _lock.CheckLockout(user);

                await ReplyAsync(returnString.ToString());
            }
            catch (Exception ex)
            {
                log = new LogMessage
                {
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message,
                };

                await _logs.ManualLog(log);
                await Context.Channel.SendMessageAsync($"{nameof(Ping)} failed");
            }
        }
    }
}
