using System;
using System.Threading.Tasks;

namespace PokecordCatcherBot
{
    public class Program
    {
        static void Main() => new PokecordCatcher().MainAsync().GetAwaiter().GetResult();
    }
}
