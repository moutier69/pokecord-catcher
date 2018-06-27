using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;

namespace PokecordCatcherBot
{
    public class PokecordCatcher
    {
        public const ulong POKECORD_ID = 365975655608745985;

        public Configuration Configuration { get; private set; }
        public DiscordSocketClient Client { get; private set; }

        private PokemonComparer pokemon;
        private readonly HttpClient http = new HttpClient();
        
        public async Task MainAsync()
        {
            var hashes = new Dictionary<string, byte[]>();

            foreach (var x in JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("poke.json")))
            {
                var str = x.Value.Substring(2);

                int charsLen = str.Length;
                byte[] bytes = new byte[charsLen / 2];

                for (int i = 0; i < charsLen; i += 2)
                    bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);

                hashes.Add(x.Key, bytes);
            }

            pokemon = new PokemonComparer(hashes);

            Configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));

            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
#if DEBUG
                LogLevel = LogSeverity.Verbose,
#else
                LogLevel = LogSeverity.Info,
#endif
                WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance,
            });


            Client.Log += async x => Console.WriteLine($"[{x.Severity.ToString()}] {x.Message}");

            Client.MessageReceived += async msg =>
            {
                if (msg.Author.Id != POKECORD_ID || msg.Embeds?.Count == 0)
                    return;

                Embed embed = msg.Embeds.First();

                if (embed.Description?.Contains(Configuration.PokecordPrefix + "catch") != true || !embed.Image.HasValue)
                    return;

                Console.WriteLine("Detected pokemon, catching...");

                var watch = System.Diagnostics.Stopwatch.StartNew();

                string name = pokemon.GetPokemon(await http.GetStreamAsync(embed.Image.Value.Url));

                watch.Stop();

                Console.WriteLine($"Found pokemon in {watch.ElapsedMilliseconds}ms");

                await msg.Channel.SendMessageAsync($"{Configuration.PokecordPrefix}catch {name}");
                await msg.Channel.SendMessageAsync(":joy: :ok_hand: LE POKEMANS XDXD");
            };

            await Client.LoginAsync(TokenType.User, Configuration.Token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
