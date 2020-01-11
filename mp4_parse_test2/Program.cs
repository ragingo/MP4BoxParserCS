using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using mp4_parse_test2.Boxes;

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
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var pipe = new Pipe();
                var w = WritePipeAsync(pipe.Writer, fs);
                var r = ReadPipeAsync(pipe.Reader);
                await Task.WhenAll(w, r).ConfigureAwait(false);
            }
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

    class Mp4Container
    {
        private long _containerOffset;
        private long _bufferOffset;
        private Box _currentBox;

        private long _totalReceivedLength;

        public void ParsePartialData(ReadOnlySequence<byte> seq)
        {
            _totalReceivedLength += seq.Length;

            // 次のボックスの先頭まで受信ができてなかったら抜ける
            if (_containerOffset > _totalReceivedLength)
            {
                return;
            }

            long startOffset = seq.Length - (_totalReceivedLength - _containerOffset);

            _bufferOffset = startOffset;

            while (_bufferOffset < seq.Length)
            {
                var span = seq.Slice(_bufferOffset).FirstSpan;

                _currentBox = new Box(span);
                _bufferOffset += _currentBox.Size;
                Console.WriteLine(_currentBox.ToString());

                switch ((BoxType)_currentBox.Type)
                {
                    case BoxType.ftyp:
                        {
                            //var ftyp = new FileTypeBox(ref _currentBox, span.Slice(_offset));
                        }
                        break;

                    default:
                        break;
                }
            }

            _containerOffset += _bufferOffset;
        }
    }

    static class BoxSizes
    {
        public static readonly int box = Marshal.SizeOf<Box>();
        public static readonly int ftyp = Marshal.SizeOf<FileTypeBox>();
    }

    static class BoxUtils
    {
        public static string GetStringFromUInt32(uint value)
        {
            Span<char> chars = stackalloc char[4];
            chars[0] = (char)(value >> 24 & 0xff);
            chars[1] = (char)(value >> 16 & 0xff);
            chars[2] = (char)(value >> 8 & 0xff);
            chars[3] = (char)(value & 0xff);
            return new string(chars);
        }

        public static string ToString(Box box)
        {
            return string.Format("type: {0}, size: 0x{1:x2}", GetStringFromUInt32(box.Type), box.Size);
        }

        public static string ToString(FileTypeBox box)
        {
            return string.Format("type: {0}, size: 0x{1:x2}({1:#,#})", GetStringFromUInt32(box.Box.Type), box.Box.Size);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Box
    {
        public int Size;
        public uint Type;

        public Box(ReadOnlySpan<byte> span)
        {
            Size = (int)BinaryPrimitives.ReadUInt32BigEndian(span.Slice(0, 4));
            Type = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(4, 4));
        }

        public override string ToString()
        {
            return BoxUtils.ToString(this);
        }
    }

    // ftyp
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FileTypeBox
    {
        public Box Box;
        public uint MajorBland;
        public uint MinorVersion;

        public FileTypeBox(ref Box box, ReadOnlySpan<byte> span)
        {
            Box = box;
            MajorBland = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(0, 4));
            MinorVersion = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(4, 4));
        }

        public override string ToString()
        {
            return BoxUtils.ToString(this);
        }
    }
}
