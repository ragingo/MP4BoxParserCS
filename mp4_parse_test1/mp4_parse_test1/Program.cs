using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mp4_parse_test1
{
	class Program
	{
		static void DumpBoxTree(List<Box> nodes, int level = 0)
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
			using (var br = new BinaryReader2(fs, true))
			{
				var boxes = new BoxParser().Parse(br);

				DumpBoxTree(boxes);
				ShowHandlers(fs, br, boxes);
				//ShowAudioInfo(fs, br, nodes);

				return;
			}
		}

		private static void ShowHandlers(FileStream fs, BinaryReader2 br, List<Box> boxes)
		{
			var result =
				from box1 in boxes.First(box => box.Type == BoxType.moov).Children
				where box1.Type == BoxType.trak
				let mdia = box1.GetChild(BoxType.mdia)
				let hdlr = mdia.GetChild<HdlrBox>()
				select hdlr;

			result.ToList().ForEach(Console.WriteLine);
		}

		private static void ShowAudioInfo(FileStream fs, BinaryReader2 br, List<Box> boxes)
		{
			var audio =
				from box1 in boxes.First(box => box.Type == BoxType.moov).Children
				where box1.Type == BoxType.trak
				let mdia = box1.GetChild(BoxType.mdia)
				let hdlr = mdia.GetChild<HdlrBox>()
				where hdlr.HandlerType == HandlerTypes.Sound

				let minf = mdia.GetChild(BoxType.minf)
				let stbl = minf.GetChild<StblBox>()
				let stsd = stbl.GetChild<StsdBox>()
				let mp4a = stsd.GetChild<Mp4AudioSampleEntry>()
				let esds = mp4a.GetChild<ESDescriptorBox>()
				let stts = stbl.GetChild<SttsBox>()
				let stsc = stbl.GetChild<StscBox>()
				let stsz = stbl.GetChild<StszBox>()

				select new {
					esds = esds,
					stts = stts,
					stsc = stsc,
					stsz = stsz,
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
					Console.WriteLine("index: {0:#,0}, size: {1:#,0}", item2.Index, item2.Entry.Size);
				}
			}
		}
	}
}
