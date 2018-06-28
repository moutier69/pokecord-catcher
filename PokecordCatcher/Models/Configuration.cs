using System;
using System.Collections.Generic;
using System.Text;

namespace PokecordCatcherBot.Models
{
    public class Configuration
    {
        public string Token { get; set; }
        public string PokecordPrefix { get; set; }
        public string UserbotPrefix { get; set; }
        public ulong OwnerID { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableCatchResponse { get; set; }
        public string CatchResponse { get; set; }

        public string[] WhitelistedPokemon { get; set; }
        public ulong[] WhitelistedGuilds { get; set; }
    }
}
