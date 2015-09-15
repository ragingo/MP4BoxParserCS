using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
	}
	class BitBuilder
	{
		public BitBuilder()
		{
		}

	}
	class Program
	{
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
			string[] fileNames = { @"I:\Development\Data\Video\mp4_h264_aac.mp4", @"D:\data\video\mp4_h264_aac.mp4" };
			string fileName = fileNames.First(f => File.Exists(f));

			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var container = Mp4Container.Parse(fs);

				DumpBoxTree(container.Boxes);
				//ShowHandlers(container.Boxes);
				ShowAudioInfo(container.Boxes);

				return;
			}
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

				select new {
					esds = esds,
					stts = stts,
					stsc = stsc,
					stsz = stsz,
					stco = stco,
					mp4a = mp4a,
					// TODO: Audio抽出 よくわからない
					//Test1 =
					//	from chunk in Enumerable.Range(0, (int)stsc.EntryCount)
					//	let first = (int)stsc.Entries[chunk].FirstChunk - 1
					//	let offset = stco.Entries[first].ChunkOffset
					//	from sample in Enumerable.Range(0, (int)stsc.Entries[chunk].SamplesPerChunk)
					//	let size = stsz.Entries[chunk + sample].Size
					//	select new { chunk=first+1, chunk_offset = offset, sample=chunk+sample, sample_size=size }
				};


			// AAC抽出サンプル： http://hujimi.seesaa.net/article/239922100.html
			var b = audio.First();

			//var mdat = boxes.First(x => x is MediaDataBox) as MediaDataBox;
			//byte[] data = mdat.Data.ToArray();
			string[] fileNames = { @"I:\Development\Data\Video\mp4_h264_aac.mp4", @"D:\data\video\mp4_h264_aac.mp4" };
			string fileName = fileNames.First(f => File.Exists(f));
			byte[] data = File.ReadAllBytes(fileName);

			using (var fs = new FileStream(@"D:\temp\a.aac", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				// TODO: http://www.wdic.org/w/TECH/ADTS
				// TODO: http://www.p23.nl/projects/aac-header/
				byte[] aac_header = new byte[7];
				aac_header[0] = 0xff;
				aac_header[1] = 0xf8;
				//aac_header[2] = (byte)(0x40 | ((byte)b.mp4a.SampleRate << 2) | (b.mp4a.ChannelCount >> 2));
				aac_header[2] = 0x50;
				aac_header[6] = 0xfc;

				int total_sample_count = 0;

				for (int chunk_idx = 0; chunk_idx < b.stsc.EntryCount; chunk_idx++)
				{
					int first_chunk_idx = (int)b.stsc.Entries[chunk_idx].FirstChunk - 1;
					int chunk_offset = (int)b.stco.Entries[first_chunk_idx].ChunkOffset; // mp4ファイル内オフセット！mdat BOX 内オフセットじゃない！！
					int sample_offset = 0;

					for (int sample_idx = 0; sample_idx < b.stsc.Entries[chunk_idx].SamplesPerChunk; sample_idx++)
					{
						uint sample_size = b.stsz.Entries[total_sample_count].Size;
						uint file_size = sample_size + 7;
						aac_header[3] = (byte)((b.mp4a.ChannelCount << 6) | (byte)(file_size >> 11));
						aac_header[4] = (byte)(file_size >> 3);
						aac_header[5] = (byte)((file_size << 5) | (0x7ff >> 6));

						fs.Write(aac_header, 0, aac_header.Length);
						fs.Write(data, chunk_offset + sample_offset, (int)sample_size);

						var result = new { 
							chunk = first_chunk_idx + 1,
							chunk_offset = chunk_offset,
							sample = total_sample_count + 1,
							sample_size = sample_size,
							aac_header = "0x"+string.Join("",aac_header.Select(x=>x.ToString("X2"))),
							sample_offset
						};
						Console.WriteLine("chunk = {0,5}, chunk_offset = {1,7}, sample = {2,4}, sample_size = {3,4}, aac_header = {4,16}, sample_offset = {5, 4}", 
							result.chunk, result.chunk_offset, result.sample, result.sample_size, result.aac_header, result.sample_offset);

						total_sample_count++;
						sample_offset += (int)sample_size;
					}
				}
			}

			foreach (var item in audio)
			{
				Console.WriteLine(item);
				//foreach (var item2 in item.stts.Entries.Select((x, i) => new { Index = i + 1, Entry = x }))
				//{
				//	Console.WriteLine("index: {0:#,0}, count: {1:#,0}, delta: {2:#,0}", item2.Index, item2.Entry.SampleCount, item2.Entry.SampleDelta);
				//}
				//foreach (var item2 in item.stsc.Entries.Select((x, i) => new { Index = i + 1, Entry = x }))
				//{
				//	Console.WriteLine("index: {0:#,0}, first_chunk: {1:#,0}, sample_per_chunk: {2:#,0}, desc_index: {3:#,0}", item2.Index, item2.Entry.FirstChunk, item2.Entry.SamplesPerChunk, item2.Entry.SampleDescriptionIndex);
				//}
				//foreach (var item2 in item.stsz.Entries.Select((x, i) => new { Index = i + 1, Entry = x }))
				//{
				//	//Console.WriteLine("index: {0:#,0}, size: {1:#,0}", item2.Index, item2.Entry.Size);
				//}
			}
		}
	}
}
