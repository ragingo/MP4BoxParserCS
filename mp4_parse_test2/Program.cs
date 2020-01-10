using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;

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

            string fileName = args[0];
            if (!File.Exists(fileName))
            {
                return;
            }

            await ParseFileAsync(fileName);
        }

        private async static Task ParseFileAsync(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var pipe = new Pipe();
                var w = WritePipeAsync(pipe.Writer, fs);
                var r = ReadPipeAsync(pipe.Reader);
                await Task.WhenAll(w, r);
            }
        }

        private async static Task WritePipeAsync(PipeWriter writer, Stream stream)
        {
            while (true)
            {
                var memory = writer.GetMemory(1024);
                int len = await stream.ReadAsync(memory);
                if (len == 0)
                {
                    break;
                }

                writer.Advance(len);

                var result = await writer.FlushAsync();
                if (result.IsCompleted)
                {
                    break;
                }
            }

            await writer.CompleteAsync();
        }

        private async static Task ReadPipeAsync(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                if (result.IsCompleted)
                {
                    break;
                }

                var buffer = result.Buffer;
                if (!buffer.IsEmpty)
                {
                    // TODO: ...
                    //Console.WriteLine(string.Join(" ", buffer.Slice(0, 4).ToArray().Select(x => x.ToString("x2"))));
                }

                reader.AdvanceTo(result.Buffer.End);
            }

            await reader.CompleteAsync();
        }
    }
}
