using System;
using System.IO;
using System.Threading.Tasks;
using mp4_parse_test2.Mp4;
using mp4_parse_test2.Mp4.Types;

namespace mp4_parse_test2
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            var uri = new Uri(Path.GetFullPath(args[0]));
            using var reader = new Mp4Loader(uri);
            reader.BoxLoaded += OnBoxLoaded;
            await reader.LoadAsync().ConfigureAwait(true);
        }

        private static void OnBoxLoaded(Box box)
        {
            Console.WriteLine(box);
        }
    }
}
