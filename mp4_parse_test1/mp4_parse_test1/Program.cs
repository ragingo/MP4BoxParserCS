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
			const string fileName = @"I:\Development\Data\Video\mp4_h264_aac.mp4";
			//const string fileName = @"D:\data\video\mp4_h264_aac.mp4";

			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var br = new BinaryReader2(fs, true))
			{
				var nodes = new BoxParser().GetBoxes(br);

				DumpBoxTree(nodes);
				ShowHandlers(fs, br, nodes);
				ShowAudioInfo(fs, br, nodes);

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
				let stbl = minf.Children.First(box => box.Type == BoxType.stbl)
				let stsd = stbl.Children.First(box => box.Type == BoxType.stsd)
				let mp4a = stsd.Children.First(box => box.Type == BoxType.mp4a)
				let esds = mp4a.Children.First(box => box.Type == BoxType.esds)

				let Mp4aBoxInfo = new
				{
					// esds box
					DecoderConfigDescriptor = new
					{
						Channels             = fs.Seek(mp4a.Offset + 24, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadUInt16()) : "error!",
						BitPerSample         = fs.Seek(mp4a.Offset + 26, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadUInt16()) : "error!",
						SampleRate           = fs.Seek(mp4a.Offset + 30, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadUInt32()) : "error!",
						ObjectTypeIndication = fs.Seek(mp4a.Offset + 55, SeekOrigin.Begin) != 0 ? string.Format("{0:X}",   br.ReadByte())   : "error!",
						MaxBitRate           = fs.Seek(mp4a.Offset + 60, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadInt32())  : "error!",
						AvgBitRate           = fs.Seek(mp4a.Offset + 64, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadInt32())  : "error!"
					}
				}

				let stts = stbl.Children.First(box => box.Type == BoxType.stts)

				let DecodingTimeToSampleBoxInfo = new
				{
					Dummy = fs.Seek(stts.Offset, SeekOrigin.Begin),
					Dummy2 = fs.Seek(stts.Offset + 8 + 4, SeekOrigin.Begin),
					Entries =
						from ent in Enumerable.Range(0, (int)br.ReadUInt32())
						select new
						{
							_Index = ent+1,
							SampleCount = fs.Seek(stts.Offset + 8 + 4 + 4 + (ent * 4 * 2 + 0), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
							SampleDelta = fs.Seek(stts.Offset + 8 + 4 + 4 + (ent * 4 * 2 + 4), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
						}
				}

				let stsc = stbl.Children.First(box => box.Type == BoxType.stsc)

				let SampleToChunkBoxInfo = new
				{
					Dummy = fs.Seek(stsc.Offset, SeekOrigin.Begin),
					Dummy2 = fs.Seek(stsc.Offset + 8 + 4, SeekOrigin.Begin),
					Entries =
						from ent in Enumerable.Range(0, (int)br.ReadUInt32())
						select new
						{
							_Index = ent+1,
							FirstChunk = fs.Seek(stsc.Offset + 8 + 4 + 4 + (ent * 4 * 3 + 0), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
							SamplePerChunk = fs.Seek(stsc.Offset + 8 + 4 + 4 + (ent * 4 * 3 + 4), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
							SampleDescriptionIndex = fs.Seek(stsc.Offset + 8 + 4 + 4 + (ent * 4 * 3 + 8), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
							//FirstChunk = br.ReadUInt32(), // 何故か上のと絡むと、相対位置からの移動が狂う。。。
							//SamplePerChunk = br.ReadUInt32(),
							//SampleDescriptionIndex = br.ReadUInt32(),
						}
				}

				select new {
					Mp4aBoxInfo = Mp4aBoxInfo,
					DecodingTimeToSampleBoxInfo = DecodingTimeToSampleBoxInfo,
					SampleToChunkBoxInfo = SampleToChunkBoxInfo,
				};

			foreach (var item in audio)
			{
				Console.WriteLine(item);
				foreach (var item2 in item.DecodingTimeToSampleBoxInfo.Entries)
				{
					Console.WriteLine(item2);
				}
				foreach (var item2 in item.SampleToChunkBoxInfo.Entries)
				{
					Console.WriteLine(item2);
				}
			}
		}
	}
}
