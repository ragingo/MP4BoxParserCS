using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using mp4_parse_test1.Boxes;

namespace mp4_parse_test1
{
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
			using (var br = new BinaryReader(fs, true))
			{
				var parser = new BoxParser(br);
				var boxes = parser.Parse();

				DumpBoxTree(boxes);
				ShowHandlers(fs, br, boxes);
				//ShowAudioInfo(fs, br, boxes);

				return;
			}
		}

		private static void ShowHandlers(FileStream fs, BinaryReader br, IEnumerable<Box> boxes)
		{
			var result =
				from box1 in boxes.First(box => box.Type == BoxType.moov).Children
				where box1.Type == BoxType.trak
				let mdia = box1.GetChild(BoxType.mdia)
				let hdlr = mdia.GetChild<HandlerBox>()
				select hdlr;

			result.ToList().ForEach(Console.WriteLine);
		}

		private static void ShowAudioInfo(FileStream fs, BinaryReader br, IEnumerable<Box> boxes)
		{
			var audio =
				from box1 in boxes.First(box => box.Type == BoxType.moov).Children
				where box1.Type == BoxType.trak
				let mdia = box1.GetChild(BoxType.mdia)
				let hdlr = mdia.GetChild<HandlerBox>()
				where hdlr.HandlerType == HandlerType.Sound

				let minf = mdia.GetChild<MediaInformationBox>()
				let stbl = minf.GetChild<SampleTableBox>()
				let stsd = stbl.GetChild<SampleDescriptionBox>()
				let mp4a = stsd.GetChild<Mp4AudioSampleEntry>()
				let esds = mp4a.GetChild<ESDescriptorBox>()
				let stts = stbl.GetChild<TimeToSampleBox>()
				let stsc = stbl.GetChild<StscBox>()
				let stsz = stbl.GetChild<StszBox>()
				let stco = stbl.GetChild<StcoBox>()

				select new {
					esds = esds,
					stts = stts,
					stsc = stsc,
					stsz = stsz,
					stco = stco,
					// TODO: Audio抽出 よくわからない
					//Test1 =
					//	from chunk_entry_index in Enumerable.Range(0, (int)stsc.EntryCount)
					//	from chunk_index in Enumerable.Range((int)stsc.Entries[chunk_entry_index].FirstChunk, (int)stco.EntryCount)
					//	from sample_to_chunk_entry_index in Enumerable.Range(0, (int)stsc.EntryCount)
					//	select stsc.Entries[sample_to_chunk_entry_index].SamplesPerChunk
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
	}
}
