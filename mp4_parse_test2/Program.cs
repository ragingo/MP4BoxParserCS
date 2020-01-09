using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

// 
// .NET Core 3.1 版
// コンパイルエラーが出て動かない！ (mac dotnet --version -> 3.1.100)
// 

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
                // Program.cs(42,50): error CS1503: 引数 1: は 'System.Memory<byte>' から 'byte[]' へ変換することはできません。
                int len = await stream.ReadAsync(memory, (int)stream.Position, 1024);
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

                break;
            }

            await writer.CompleteAsync();
        }

        private async static Task ReadPipeAsync(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;

                Console.WriteLine(buffer.Slice(0, 4));

                if (result.IsCompleted)
                {
                    break;
                }
            }

            await reader.CompleteAsync();
        }
    }
}
