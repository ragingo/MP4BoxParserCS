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
				from node in nodes
					from child2 in node.Children
					where child2.Type == BoxType.Trak
						from child3 in child2.Children
						where child3.Type == BoxType.Mdia
							from child4 in child3.Children
							where child4.Type == BoxType.Hdlr
							select new
							{
								HandlerType = fs.Seek(child4.Offset + 16, SeekOrigin.Begin) != 0 ? StringUtils.FromBinary(br.ReadUInt32()) : "error!"
							};

			result.ToList().ForEach(Console.WriteLine);
		}

		private static void ShowAudioInfo(FileStream fs, BinaryReader2 br, List<BoxNode> nodes)
		{
			var audio =
				from node in nodes
					from box1 in node.Children
					where box1.Type == BoxType.Trak
					select box1 into trak
						from box2 in trak.Children
						where box2.Type == BoxType.Mdia
						select new
						{
							Box = box2,
							HandlerType =
								(from box3 in box2.Children
									where box3.Type == BoxType.Hdlr
									select fs.Seek(box3.Offset + 16, SeekOrigin.Begin) != 0 ? StringUtils.FromBinary(br.ReadUInt32()) : "error!"
								).FirstOrDefault()
						} into mdia_type
						where mdia_type.HandlerType == HandlerTypes.Sound
							from box4 in mdia_type.Box.Children
							where box4.Type == BoxType.Minf
							select box4 into minf
								from box5 in minf.Children
								where box5.Type == BoxType.Stbl
								select new
								{
									StblBox = box5,
									Mp4aBoxInfo =
										(from box6 in box5.Children
											where box6.Type == BoxType.Stsd
											select box6 into stsd
											from box7 in stsd.Children
											where box7.Type == BoxType.Mp4a
											select box7 into mp4a
												from box8 in mp4a.Children
												where box8.Type == BoxType.Esds
												select new
												{
													DecoderConfigDescriptor = new
													{
														Channels = fs.Seek(mp4a.Offset + 24, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadUInt16()) : "error!",
														BitPerSample = fs.Seek(mp4a.Offset + 26, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadUInt16()) : "error!",
														SampleRate = fs.Seek(mp4a.Offset + 30, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadUInt32()) : "error!",
														ObjectTypeIndication = fs.Seek(mp4a.Offset + 55, SeekOrigin.Begin) != 0 ? string.Format("{0:X}", br.ReadByte()) : "error!",
														MaxBitRate = fs.Seek(mp4a.Offset + 60, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadInt32()) : "error!",
														AvgBitRate = fs.Seek(mp4a.Offset + 64, SeekOrigin.Begin) != 0 ? string.Format("{0:#,#}", br.ReadInt32()) : "error!"
													}
												}
										).FirstOrDefault(),
									DecodingTimeToSampleBoxInfo =
										(from box6 in box5.Children
											where box6.Type == BoxType.Stts
											select new
											{
											Dummy = fs.Seek(box6.Offset, SeekOrigin.Begin),
											Dummy2 = fs.Seek(box6.Offset + 8 + 4, SeekOrigin.Begin),
											Entries =
												from ent in Enumerable.Range(0, (int)br.ReadUInt32())
												select new
												{
													_Index = ent+1,
													SampleCount = fs.Seek(box6.Offset + 8 + 4 + 4 + (ent * 4 * 2 + 0), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
													SampleDelta = fs.Seek(box6.Offset + 8 + 4 + 4 + (ent * 4 * 2 + 4), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
												}
											}).FirstOrDefault(),
									SampleToChunkBoxInfo =
										(from box6 in box5.Children
											where box6.Type == BoxType.Stsc
											select new
											{
											Dummy = fs.Seek(box6.Offset, SeekOrigin.Begin),
											Dummy2 = fs.Seek(box6.Offset + 8 + 4, SeekOrigin.Begin),
											Entries =
												from ent in Enumerable.Range(0, (int)br.ReadUInt32())
												select new
												{
													_Index = ent+1,
													FirstChunk = fs.Seek(box6.Offset + 8 + 4 + 4 + (ent * 4 * 3 + 0), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
													SamplePerChunk = fs.Seek(box6.Offset + 8 + 4 + 4 + (ent * 4 * 3 + 4), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
													SampleDescriptionIndex = fs.Seek(box6.Offset + 8 + 4 + 4 + (ent * 4 * 3 + 8), SeekOrigin.Begin) != 0 ? br.ReadUInt32() : 0,
													//FirstChunk = br.ReadUInt32(), // 何故か上のと絡むと、相対位置からの移動が狂う。。。
													//SamplePerChunk = br.ReadUInt32(),
													//SampleDescriptionIndex = br.ReadUInt32(),
												}
											}).FirstOrDefault()
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
