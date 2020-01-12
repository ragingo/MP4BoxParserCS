using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using mp4_parse_test2.Mp4;

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

            await ParseFileAsync(fileName).ConfigureAwait(false);
        }

        private static async Task ParseFileAsync(string fileName)
        {
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var pipe = new Pipe();
            var w = WritePipeAsync(pipe.Writer, fs);
            var r = ReadPipeAsync(pipe.Reader);
            await Task.WhenAll(w, r).ConfigureAwait(false);
        }

        private static async Task WritePipeAsync(PipeWriter writer, Stream stream)
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

        private static async Task ReadPipeAsync(PipeReader reader)
        {
            var mp4 = new Mp4Container();

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
                    mp4.ParsePartialData(buffer);
                }

                reader.AdvanceTo(result.Buffer.End);
            }

            await reader.CompleteAsync();
        }
    }
}
