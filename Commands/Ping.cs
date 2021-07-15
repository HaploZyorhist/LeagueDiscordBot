#nullable enable
using Discord.Commands;
using Discord.WebSocket;
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
        private readonly DiscordSocketClient _client;

        public Ping (LogService logs, LockOutService lockout, DiscordSocketClient client)
        {
            _logs = logs;
            _lock = lockout;
            _client = client;
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
                        returnString.AppendLine($" Pong!");
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

                    returnString.AppendLine("Pong!");
                }
                else
                {
                    throw new Exception($"{nameof(Ping)} was called incorrectly");
                }

                var lockCheck = _lock.CheckLockout(user);

                // TODO: This is the functionality for pinging all servers.  Will be used for raid bosses
                foreach (var server in _client.Guilds)
                {
                    foreach (var channel in server.Channels)
                    {
                        string returnStr = "Pong!";
                            ulong channelLong = ulong.Parse(channel.Id.ToString());
                            var chn = _client.GetChannel(channelLong) as Discord.IMessageChannel;

                        if (chn != null)
                        {
                            await chn.SendMessageAsync(returnStr);
                        }
                    }
                }

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
