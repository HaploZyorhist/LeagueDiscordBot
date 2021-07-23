#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Modules.Response
{
    /// <summary>
    /// container for handling lockout return
    /// </summary>
    public class LockOutResponse
    {
        /// <summary>
        /// boolean showing if the user is locked out
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// action the player is currently doing
        /// </summary>
        public string? Action { get; set; }
    }
}
