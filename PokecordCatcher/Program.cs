using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PokecordCatcherBot
{
    public class Program
    {
        static void Main()
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

            new PokecordCatcher(hashes).Run().GetAwaiter().GetResult();
        }
    }
}
