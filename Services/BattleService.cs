using Interactivity;
using LeagueDiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Services
{
    public class BattleService
    {
        #region Fields

        private readonly LockOutService _lockout;
        private readonly ChampionService _champ;
        private readonly LogService _log;
        private readonly InteractivityService _interact;

        #endregion

        #region CTOR

        public BattleService(LockOutService lockOut,
                             ChampionService champ,
                             LogService log,
                             InteractivityService interact)
        {
            _lockout = lockOut;
            _champ = champ;
            _log = log;
            _interact = interact;
        }

        #endregion

        #region Battle

        public async Task<Discord.IUser> Battle (Discord.IUser player1, Discord.IUser player2, Discord.IMessageChannel channel)
        {
            string action = "Battling";
   
            try
            {
                await _lockout.LockUser(player1, action);
                await _lockout.LockUser(player2, action);

                await channel.SendMessageAsync("a battle has begun");

                // this will need to become list of characters that are unused
                Discord.IUser victor = null;
                DateTime actionTimer = DateTime.Now.AddSeconds(30);

                InteractivityResult<Discord.WebSocket.SocketMessage> chosenAction = null;

                var rand = new Random();
                var randomPlayer = rand.Next(1, 3);


                Discord.IUser activePlayer = null;
                Discord.IUser passivePlayer = null;

                if (randomPlayer == 2)
                {
                    activePlayer = player1;
                    passivePlayer = player2;
                }
                else if (randomPlayer == 1)
                {
                    activePlayer = player2;
                    passivePlayer = player1;
                }

                int turnCounter = 0;

                while (victor == null)
                {
                    turnCounter++;
                    actionTimer = DateTime.Now.AddSeconds(30);
                    // start of turn stuff
                    // get list of champions for Active player
                    // give the player a chance to use each of their champions
                    // resolve the turn

                    var randomNumber = rand.Next(1, 11);

                    if (activePlayer == player2)
                    {
                        activePlayer = player1;
                        passivePlayer = player2;
                    }
                    else if (activePlayer == player1)
                    {
                        activePlayer = player2;
                        passivePlayer = player1;
                    }

                    bool messageValid = false;
                    int goodGuess = 0;

                    while (!messageValid)
                    {
                        await channel.SendMessageAsync($"{activePlayer.Mention} please choose a number between 1 and 10");
                        var remainingTime = (actionTimer - DateTime.Now).TotalSeconds;
                        chosenAction = await _interact.NextMessageAsync(x => (x.Author.Id == player1.Id || x.Author.Id == player2.Id) &&
                                                                          x.Channel == channel, null, TimeSpan.FromSeconds(remainingTime));

                        if (chosenAction.Value == null ||
                           chosenAction.IsTimeouted == true)
                        {
                            throw new Exception("You failed to select an action in the allotted time, the battle has ended");
                        }

                        var choiceAction = chosenAction.Value.ToString().ToLower();

                        if ((chosenAction.Value.Author.Id == player1.Id && choiceAction == "!cancel") ||
                            (chosenAction.Value.Author.Id == player2.Id && choiceAction == "!cancel"))
                        {
                            throw new Exception("The battle was canceled");
                        }

                        if (chosenAction.Value.Author.Id == activePlayer.Id && 
                            int.TryParse(choiceAction, out int guess))
                        {
                            goodGuess = guess;
                            messageValid = true;
                        }
                    }

                    if (goodGuess == randomNumber)
                    {
                        victor = activePlayer;
                    }

                    // check winconditions and escape if true

                }

                return victor;
            }
            catch(Exception ex)
            {
                await _log.ManualLog(new LogMessage
                {
                    User = ulong.Parse(player1.Id.ToString()),
                    TriggerChannel = channel.Name,
                    TriggerClass = nameof(BattleService),
                    TriggerFunction = nameof(Battle),
                    Severity = Discord.LogSeverity.Error,
                    Message = ex.Message
                });

                await channel.SendMessageAsync("battle has ended");

                return null;
            }
        }

        #endregion
    }
}
