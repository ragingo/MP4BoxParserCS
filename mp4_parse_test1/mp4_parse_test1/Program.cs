using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mp4_parse_test1
{
	class Program
	{
		static void DumpBoxTree(List<BoxNode> nodes, int level = 0)
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
				var nodes = new BoxParser().GetBoxes(br);

				DumpBoxTree(nodes);
				ShowHandlers(fs, br, nodes);
				//ShowAudioInfo(fs, br, nodes);

				return;
			}
		}

		private static void ShowHandlers(FileStream fs, BinaryReader2 br, List<BoxNode> nodes)
		{
			var result =
				from box1 in nodes.First(box => box.Type == BoxType.moov).Children
				where box1.Type == BoxType.trak
				let mdia = box1.Children.First(box => box.Type == BoxType.mdia)
				let hdlr = mdia.Children.First(box => box.Type == BoxType.hdlr) as HdlrBoxNode
				where hdlr.HandlerType == HandlerTypes.Sound || 
					  hdlr.HandlerType == HandlerTypes.Video
				select hdlr;

			result.ToList().ForEach(Console.WriteLine);
		}

		private static void ShowAudioInfo(FileStream fs, BinaryReader2 br, List<BoxNode> nodes)
		{
			var audio =
				from box1 in nodes.First(box => box.Type == BoxType.moov).Children
				where box1.Type == BoxType.trak
				let mdia = box1.Children.First(box => box.Type == BoxType.mdia)
				let hdlr = mdia.Children.First(box => box.Type == BoxType.hdlr) as HdlrBoxNode
				where hdlr.HandlerType == HandlerTypes.Sound

				let minf = mdia.Children.First(box => box.Type == BoxType.minf)
				let stbl = minf.Children.First(box => box.Type == BoxType.stbl) as StblBoxNode
				let stsd = stbl.Children.First(box => box.Type == BoxType.stsd) as StsdBoxNode
				let mp4a = stsd.Children.First(box => box.Type == BoxType.mp4a) as Mp4AudioSampleEntryNode
				let esds = mp4a.Children.First(box => box.Type == BoxType.esds) as ESDescriptorBoxNode
				let stts = stbl.Children.First(box => box.Type == BoxType.stts) as SttsBoxNode
				let stsc = stbl.Children.First(box => box.Type == BoxType.stsc) as StscBoxNode

				select new {
					esds = esds,
					stts = stts,
					stsc = stsc,
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
			}
		}
	}
}
