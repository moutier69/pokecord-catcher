using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace PokecordCatcherBot
{
    public class PokemonComparer
    {
        private readonly ReadOnlyDictionary<string, byte[]> hashes;
        
        public PokemonComparer(Dictionary<string, byte[]> pokemonHashes)
        {
            hashes = new ReadOnlyDictionary<string, byte[]>(pokemonHashes);
        }

        public string GetPokemon(byte[] image)
        {
            Bitmap bitmap;

            using (var stream = new MemoryStream(image))
                bitmap = (Bitmap)Image.FromStream(stream);

            var hash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());

            return Compare(hash);
        }

        public string GetPokemon(Stream image)
        {
            var bitmap = (Bitmap)Image.FromStream(image);
            var hash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());

            return Compare(hash);
        }

        private string Compare(Digest hash)
        {
            Dictionary<string, double> similarities = new Dictionary<string, double>();

            foreach (var x in hashes)
            {
                var correlation = ImagePhash.GetCrossCorrelation(hash.Coefficents, x.Value);

                if (correlation >= 1.0)
                {
                    Console.WriteLine($"Detected '{x.Key}' with a similarity of {correlation}");
                    return x.Key;
                }

                similarities.Add(x.Key, correlation);
            }

            var sim = similarities.OrderByDescending(x => x.Value).First();
            Console.WriteLine($"Detected '{sim.Key}' with a similarity of {sim.Value}");

            return sim.Key;
        }
    }
}
