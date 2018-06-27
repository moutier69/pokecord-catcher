using Newtonsoft.Json;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace HashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var bitmap = (Bitmap)Image.FromFile(args[0]);
                var hash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage()).ToString();

                Console.WriteLine("Hash:");
                Console.WriteLine(hash);

                return;
            }

            Console.WriteLine("Enter a path to the directory of Pokemon that you want to hash:");
            var dirName = Console.ReadLine();
            var files = Directory.GetFiles(dirName);
            var hashes = new ConcurrentDictionary<string, string>();

            Parallel.ForEach(files, (x) =>
            {
                Console.WriteLine("Processing " + x);

                var bitmap = (Bitmap)Image.FromFile(x);
                var hash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage()).ToString();

                hashes.AddOrUpdate(Path.GetFileNameWithoutExtension(x), hash, (k, v) => v);
            });

            File.WriteAllText("poke.json", JsonConvert.SerializeObject(hashes));
        }
    }
}
