using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDiscordBot.Modules
{
    /// <summary>
    /// container for handling lockout return
    /// </summary>
    public class LockOut
    {
        /// <summary>
        /// boolean showing if the user is locked out
        /// </summary>
        public bool Locked { get; set; }
    }
}
