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
using Discord.Rest;

namespace PokecordCatcherBot
{
    public class PokecordCatcher
    {
        public const ulong POKECORD_ID = 365975655608745985;

        public Configuration Configuration { get; }
        public DiscordSocketClient Client { get; }

        private readonly HttpClient http = new HttpClient();
        private readonly PokemonComparer pokemon;
        private readonly ResponseGrabber responseGrabber;

        public PokecordCatcher(Dictionary<string, byte[]> pokemonHashes)
        {
            pokemon = new PokemonComparer(pokemonHashes);

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

            responseGrabber = new ResponseGrabber(Client);

            if (Configuration.EnableLogging)
                Logger.StartLogging();

            Client.Log += Log;
            Client.MessageReceived += async x => Task.Run(async () => await OnMessage(x));
        }

        private async Task Log(LogMessage x) => Console.WriteLine($"[{x.Severity.ToString()}] {x.Message}");

        private async Task OnMessage(SocketMessage msg)
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

            var resp = await responseGrabber.SendMessageAndGrabResponse(
                (ITextChannel)msg.Channel,
                $"{Configuration.PokecordPrefix}catch {name}",
                x => x.Channel.Id == msg.Channel.Id && x.Author.Id == POKECORD_ID && x.MentionedUsers.Any(y => y.Id == Client.CurrentUser.Id) && x.Content.StartsWith("Congratulations"),
                5
            );

            Console.WriteLine(resp == null ? "The Pokecord bot did not respond, catch was a fail." : "Catch confirmed by the Pokecord bot.");

            if (resp != null)
            {
                if (Configuration.EnableCatchResponse)
                    await msg.Channel.SendMessageAsync(Configuration.CatchResponse);

                Logger.Log($"Caught a {name} in #{resp.Channel.Name} ({((SocketGuildChannel)resp.Channel).Guild.Name})");
            }
            else
            {
                Logger.Log($"Failed to catch {name} in #{resp.Channel.Name} ({((SocketGuildChannel)resp.Channel).Guild.Name})");
            }

            Console.WriteLine();
        }

        public async Task Run()
        {
            await Client.LoginAsync(TokenType.User, Configuration.Token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
