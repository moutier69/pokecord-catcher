using System;
using System.Collections.Generic;
using System.Text;

namespace PokecordCatcherBot
{
    public class Configuration
    {
        public string Token { get; set; }
        public string PokecordPrefix { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableCatchResponse { get; set; }
        public string CatchResponse { get; set; }
    }
}
