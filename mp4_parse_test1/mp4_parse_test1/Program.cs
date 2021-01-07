using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using mp4_parse_test1.Boxes;

namespace mp4_parse_test1
{
    struct Bit
    {
        public static readonly Bit On = new Bit(true);
        public static readonly Bit Off = new Bit(false);
        private bool _bit;
        private Bit(bool bit)
        {
            _bit = bit;
        }
        public static implicit operator Bit(int a)
        {
            return new Bit(a != 0);
        }
        public static implicit operator int(Bit a)
        {
            return a._bit ? 1 : 0;
        }
        public override string ToString()
        {
            return _bit ? "1" : "0";
        }

        public static readonly uint[] Masks = new uint[] {
            0x00000001, 0x00000003, 0x00000007, 0x0000000f,
            0x0000001f, 0x0000003f, 0x0000007f, 0x000000ff,
            0x000001ff, 0x000003ff, 0x000007ff, 0x00000fff,
            0x00001fff, 0x00003fff, 0x00007fff, 0x0000ffff,
            0x0001ffff, 0x0003ffff, 0x0007ffff, 0x000fffff,
            0x000fffff, 0x003fffff, 0x007fffff, 0x00ffffff,
            0x01ffffff, 0x03ffffff, 0x07ffffff, 0x0fffffff,
            0x1fffffff, 0x3fffffff, 0x7fffffff, 0xffffffff,
        };
    }
    class BitBuilder
    {
        private IEnumerable<Bit> _bits;
        public BitBuilder(IEnumerable<Bit> bits)
        {
            _bits = bits;
        }

        public byte ToByte()
        {
            if (_bits.Count() < 8)
            {
                throw new Exception();
            }
            return (byte)_bits.Take(8).Select((b, i) => new { b, i }).Select(x => (int)Math.Pow(2, 8 - x.i) * (int)x.b).Aggregate((x, y) => x | y);
        }
    }

    class Program
    {
        private static readonly Tuple<byte, int>[] SamplingFrequencies = new[] {
            new Tuple<byte, int>(0x00, 96000),
            new Tuple<byte, int>(0x01, 88200),
            new Tuple<byte, int>(0x02, 64000),
            new Tuple<byte, int>(0x03, 48000),
            new Tuple<byte, int>(0x04, 44100),
            new Tuple<byte, int>(0x05, 32000),
            new Tuple<byte, int>(0x06, 24000),
            new Tuple<byte, int>(0x07, 22050),
            new Tuple<byte, int>(0x08, 16000),
            new Tuple<byte, int>(0x09, 12000),
            new Tuple<byte, int>(0x0a, 11025),
            new Tuple<byte, int>(0x0b,  8000),
            new Tuple<byte, int>(0x0c,  7350),
            new Tuple<byte, int>(0x0d,     0),
            new Tuple<byte, int>(0x0e,     0),
            new Tuple<byte, int>(0x0f,     0),
        };

        static void DumpBoxTree(IEnumerable<Box> nodes, int level = 0)
        {
            foreach (var node in nodes)
            {
                string indent = level == 0 ? "" : " ".PadRight(level * 4);
                Console.WriteLine("{0}{1}", indent, node);

                if (node.Children.Count > 0)
                {
                    DumpBoxTree(node.Children, level + 1);
                }
            }
        }

        static void Main(string[] args)
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

            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var br = new BinaryReader(fs, true))
            {
                var container = Mp4Container.Parse(br);

                DumpBoxTree(container.Boxes);
                //ShowHandlers(container.Boxes);
                //ShowAudioInfo(container.Boxes);
                ExtractAudio(container.Boxes, fs);

                return;
            }

            //Console.WriteLine(new BitBuilder(new Bit[]{1,2,3,4,5,6,7,8,9,0}).ToByte().ToString("X")); // TODO: バグで FE になってる
        }

        private static void ShowHandlers(IEnumerable<Box> boxes)
        {
            var result =
                from box1 in boxes.First(box => box is MovieBox).Children
                where box1.Type == BoxType.trak
                let mdia = box1.GetChild<MediaBox>()
                let hdlr = mdia.GetChild<HandlerBox>()
                select hdlr;

            result.ToList().ForEach(Console.WriteLine);
        }

        private static void ShowAudioInfo(IEnumerable<Box> boxes)
        {
            var audio =
                from box1 in boxes.First(box => box is MovieBox).Children
                where box1.Type == BoxType.trak
                let mdia = box1.GetChild<MediaBox>()
                let hdlr = mdia.GetChild<HandlerBox>()
                where hdlr.HandlerType == HandlerType.Sound

                let minf = mdia.GetChild<MediaInformationBox>()
                let stbl = minf.GetChild<SampleTableBox>()
                let stsd = stbl.GetChild<SampleDescriptionBox>()
                let mp4a = stsd.GetChild<Mp4AudioSampleEntry>()
                let esds = mp4a.GetChild<EsdBox>()
                let stts = stbl.GetChild<TimeToSampleBox>()
                let stsc = stbl.GetChild<SampleToChunkBox>()
                let stsz = stbl.GetChild<SampleSizeBox>()
                let stco = stbl.GetChild<ChunkOffsetBox>()

                select new
                {
                    esds = esds,
                    stts = stts,
                    stsc = stsc,
                    stsz = stsz,
                    stco = stco,
                    mp4a = mp4a,
                };

            foreach (var item in audio)
            {
                Console.WriteLine(item);
                foreach (var item2 in item.stts.Entries.Select((x, i) => new { Index = i + 1, Entry = x }))
                {
                    Console.WriteLine("index: {0:#,0}, count: {1:#,0}, delta: {2:#,0}", item2.Index, item2.Entry.SampleCount, item2.Entry.SampleDelta);
                }
                foreach (var item2 in item.stsc.Entries.Select((x, i) => new { Index = i + 1, Entry = x }))
                {
                    Console.WriteLine("index: {0:#,0}, first_chunk: {1:#,0}, sample_per_chunk: {2:#,0}, desc_index: {3:#,0}", item2.Index, item2.Entry.FirstChunk, item2.Entry.SamplesPerChunk, item2.Entry.SampleDescriptionIndex);
                }
                foreach (var item2 in item.stsz.Entries.Select((x, i) => new { Index = i + 1, Entry = x }))
                {
                    //Console.WriteLine("index: {0:#,0}, size: {1:#,0}", item2.Index, item2.Entry.Size);
                }
            }
        }

        private static void ExtractAudio(IEnumerable<Box> boxes, Stream stream)
        {
            var audio =
                (from box1 in boxes.First(box => box is MovieBox).Children
                 where box1.Type == BoxType.trak
                 let mdia = box1.GetChild<MediaBox>()
                 let hdlr = mdia.GetChild<HandlerBox>()
                 where hdlr.HandlerType == HandlerType.Sound

                 let minf = mdia.GetChild<MediaInformationBox>()
                 let stbl = minf.GetChild<SampleTableBox>()
                 let stsd = stbl.GetChild<SampleDescriptionBox>()
                 let mp4a = stsd.GetChild<Mp4AudioSampleEntry>()
                 let stsc = stbl.GetChild<SampleToChunkBox>()
                 let stsz = stbl.GetChild<SampleSizeBox>()
                 let stco = stbl.GetChild<ChunkOffsetBox>()

                 select new
                 {
                     stsc = stsc,
                     stsz = stsz,
                     stco = stco,
                     mp4a = mp4a,
                 }).First();

            using (var fs = new FileStream(@"D:\temp\a.aac", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                // TODO: http://www.wdic.org/w/TECH/ADTS
                // TODO: http://www.p23.nl/projects/aac-header/

                byte freq = SamplingFrequencies.FirstOrDefault(x => x.Item2 == audio.mp4a.SampleRate).Item1;

                byte[] aac_header = new byte[7];
                aac_header[0] = 0xff; // 11111111
                aac_header[1] = 0xf9; // 1111 1 00 1
                aac_header[2] = (byte)(0x40 | (freq << 2) | (audio.mp4a.ChannelCount >> 2)); // 01 XXXX 0 X
                aac_header[6] = 0xfc; // XXXXXX 00

                int total_sample_count = 0;

                // TODO: 
                // stsc には無い chunk が stco に有る。
                // stco の chunk が stsc に無かったら 直前のサンプル数を使用する。
                // 今のところはこれで問題無かったけど、正しいかは要確認。

                int lastSampleCount = 0;

                for (int chunk_idx = 0; chunk_idx < audio.stco.EntryCount; chunk_idx++)
                {
                    int chunk_id = chunk_idx + 1;
                    int chunk_offset = (int)audio.stco.Entries[chunk_idx].ChunkOffset; // mp4ファイル内オフセット！mdat BOX 内オフセットじゃない！！
                    var chunk = audio.stsc.Entries.FirstOrDefault(x => x.FirstChunk == chunk_id);

                    int sample_count = chunk == null ? lastSampleCount : (int)chunk.SamplesPerChunk;
                    lastSampleCount = sample_count;
                    int sample_offset = 0;

                    for (int sample_idx = 0; sample_idx < sample_count; sample_idx++)
                    {
                        int sample_size = (int)audio.stsz.Entries[total_sample_count].Size;
                        int frame_size = sample_size + 7;
                        aac_header[3] = (byte)((audio.mp4a.ChannelCount << 6) | (frame_size >> 14)); // XX 0 0 0 0 XX
                        aac_header[4] = (byte)(frame_size >> 3); // XXXXXXXX
                        aac_header[5] = (byte)((frame_size << 5) | (0x7ff >> 6)); // XXX XXXXX

                        fs.Write(aac_header, 0, aac_header.Length);

                        stream.Seek(chunk_offset + sample_offset, SeekOrigin.Begin);
                        byte[] bytes = new byte[sample_size];
                        stream.Read(bytes, 0, sample_size);

                        fs.Write(bytes, 0, sample_size);

                        //ShowExtractInfo(aac_header, total_sample_count, lastSampleCount, chunk_id, chunk_offset, sample_offset, sample_size);

                        total_sample_count++;
                        sample_offset += sample_size;
                    }
                }

                Console.WriteLine("{0:#,0} samples", total_sample_count);
            }
        }

        private static void ShowExtractInfo(byte[] aac_header, int total_sample_count, int lastSampleCount, int chunk_id, int chunk_offset, int sample_offset, int sample_size)
        {
            var result = new
            {
                chunk = chunk_id,
                chunk_offset = chunk_offset,
                sample = total_sample_count + 1,
                sample_size = sample_size,
                aac_header = "0x" + string.Join("", aac_header.Select(x => x.ToString("X2"))),
                sample_offset
            };
            Console.WriteLine("chunk = {0,5}, chunk_offset = {1,7}, sample = {2,4}, sample_size = {3,4}, aac_header = {4,16}, sample_offset = {5, 4}, sample_count = {6, 6}",
                result.chunk, result.chunk_offset, result.sample, result.sample_size, result.aac_header, result.sample_offset, lastSampleCount);
        }
    }
}
