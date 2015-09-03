using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mp4_parse_test1
{
	class Program
	{
		class BoxNode
		{
			public long Offset { get; set; }
			public uint Size { get; set; }
			public BoxType Type { get; set; }
			public BoxNode Parent { get; set; }
			public List<BoxNode> Children { get; private set; }
			public int Level { get; set; }
			public bool IsRoot { get { return Level == 0; } }
			public BoxNode()
			{
				Children = new List<BoxNode>();
			}
			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(StringUtils.FromBinary((uint)Type));
				sb.AppendFormat(" {{ offset:{0:#,#}, size:{1:#,#} }}", Offset, Size);
				return sb.ToString();
			}

			public T As<T>()
				where T : BoxNode, new()
			{
				var newNode = new T();
				newNode.Offset = Offset;
				newNode.Size = Size;
				newNode.Type = Type;
				newNode.Parent = Parent;
				newNode.Children.AddRange(Children);
				newNode.Level = Level;
				return newNode;
			}
		}

		class FullBoxNode : BoxNode
		{
			public byte Version { get; set; }
			public UInt32 Flags { get; set; }

			public FullBoxNode()
			{
			}
		}

		class MvhdBoxNode : FullBoxNode
		{
			public DateTime CreationTime { get; set; }
			public DateTime ModificationTime { get; set; }
			public UInt32 TimeScale { get; set; }
			public UInt64 Duration { get; set; }
			public Double Rate { get; set; }
			public Single Volume { get; set; }
			public UInt32 NextTrackId { get; set; }

			public MvhdBoxNode()
			{
			}
		}

		class StsdBoxNode : BoxNode
		{
			public uint SampleEntries { get; set; }

			public StsdBoxNode()
			{
			}
		}

		class SampleEntryNode : BoxNode
		{
			public byte[] Reserved { get; private set; }
			public UInt16 DataReferenceIndex { get; set; }

			public SampleEntryNode()
			{
				Reserved = new byte[6];
			}
		}

		class VisualSampleEntryNode : SampleEntryNode
		{
			public UInt16 PreDefined { get; set; }
			public UInt16 Reserved2 { get; set; }
			public UInt32[] PreDefined2 { get; set; }
			public UInt16 Width { get; set; }
			public UInt16 Height { get; set; }
			public UInt32 HorizontalResolution { get; set; }
			public UInt32 VerticalResolution { get; set; }
			public UInt32 Reserved3 { get; set; }
			public UInt16 FrameCount { get; set; }
			public string CompressorName { get; set; }
			public UInt16 Depth { get; set; }
			public Int32 PreDefined3 { get; set; }

			public VisualSampleEntryNode()
			{
				PreDefined2 = new UInt32[3];
				HorizontalResolution = 0x00480000 >> 16;
				VerticalResolution = 0x00480000 >> 16;
				FrameCount = 1;
				Depth = 0x0018 >> 8;
				PreDefined3 = -1;
			}
		}

		class BoxParser
		{
			public List<BoxNode> GetBoxes(BinaryReader2 reader)
			{
				return GetBoxes(reader, null);
			}
			private List<BoxNode> GetBoxes(BinaryReader2 reader, BoxNode parent)
			{
				List<BoxNode> nodes = new List<BoxNode>();

				while (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					UInt32 boxSize = reader.ReadUInt32();
					BoxType boxType = (BoxType)reader.ReadUInt32();

					BoxNode sibling = new BoxNode();
					sibling.Offset = reader.BaseStream.Position - 4 * 2;
					sibling.Size = boxSize;
					sibling.Type = boxType;
					sibling.Parent = parent;
					nodes.Add(sibling);

					switch (boxType)
					{
					case BoxType.Moov:
					case BoxType.Trak:
					case BoxType.Mdia:
					case BoxType.Minf:
					case BoxType.Dinf:
					case BoxType.Stbl:
					case BoxType.Udta:
						sibling.Children.AddRange(GetBoxes(reader, sibling));
						break;
					case BoxType.Mvhd:
						ParseMvhd(reader, sibling);
						break;
					case BoxType.Dref:
						ParseDref(reader, sibling);
						break;
					case BoxType.Stsd:
						ParseStsd(reader, sibling);
						break;
					case BoxType.Avc1:
						ParseAvc1(reader, sibling);
						break;
					case BoxType.Mp4a:
						ParseMp4a(reader, sibling);
						break;
					default:
						reader.BaseStream.Seek((boxSize - 4 * 2), SeekOrigin.Current);
						break;
					}

					if (parent != null &&
						parent.Offset + parent.Size == sibling.Offset + sibling.Size)
					{
						return nodes;
					}
				}

				return nodes;
			}

			// Movie Header Box
			private void ParseMvhd(BinaryReader2 reader, BoxNode sibling)
			{
				var newSibling = sibling.As<MvhdBoxNode>();
				newSibling.Version = reader.ReadByte();
				newSibling.Flags = reader.ReadUInt24();
				if (newSibling.Version == 1)
				{
					newSibling.CreationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(reader.ReadUInt64());
					newSibling.ModificationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(reader.ReadUInt64());
					newSibling.TimeScale = reader.ReadUInt32();
					newSibling.Duration = reader.ReadUInt64();
				}
				else
				{
					newSibling.CreationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(reader.ReadUInt32());
					newSibling.ModificationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(reader.ReadUInt32());
					newSibling.TimeScale = reader.ReadUInt32();
					newSibling.Duration = reader.ReadUInt32();
				}
				newSibling.Rate = (Double)(reader.ReadUInt32() >> 16);
				newSibling.Volume = (Single)(reader.ReadInt16() >> 8);
				reader.ReadBytes(2); // reserved1: bit(16)
				reader.ReadBytes(4 * 2); // reserved2: unsigned int(32) [2]
				reader.ReadBytes(4 * 9); // matrix: template int(32) [9]
				reader.ReadBytes(4 * 6); // pre_defined: bit(32) [6]
				newSibling.NextTrackId = reader.ReadUInt32();
			}

			// Data Reference Box
			private void ParseDref(BinaryReader2 reader, BoxNode sibling)
			{
				reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
				reader.ReadUInt32();// entries
				sibling.Children.AddRange(GetBoxes(reader, sibling));
			}

			// Sample Description Box
			private void ParseStsd(BinaryReader2 reader, BoxNode sibling)
			{
				reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
				var newSibling = sibling.As<StsdBoxNode>();
				newSibling.SampleEntries = reader.ReadUInt32();
				sibling.Children.AddRange(GetBoxes(reader, newSibling));
			}

			// 8.5.1. p32 AudioSampleEntry
			private void ParseMp4a(BinaryReader2 reader, BoxNode sibling)
			{
				reader.BaseStream.Seek(6 + 2, SeekOrigin.Current); // SampleEntry
				reader.BaseStream.Seek(4 * 2 + 2 + 2 + 2 + 2 + 4, SeekOrigin.Current);
				sibling.Children.AddRange(GetBoxes(reader, sibling));
			}

			// 8.5.1. p31 VisualSampleEntry
			private void ParseAvc1(BinaryReader2 reader, BoxNode sibling)
			{
				var newSibling = sibling.As<VisualSampleEntryNode>();
				reader.BaseStream.Seek(6, SeekOrigin.Current); // reserved
				newSibling.DataReferenceIndex = reader.ReadUInt16();
				reader.BaseStream.Seek(2 + 2 + 4 * 3, SeekOrigin.Current); // pre_defined + reserved + predefined
				newSibling.Width = reader.ReadUInt16();
				newSibling.Height = reader.ReadUInt16();
				newSibling.HorizontalResolution = reader.ReadUInt32() >> 16;
				newSibling.VerticalResolution = reader.ReadUInt32() >> 16;
				reader.BaseStream.Seek(4, SeekOrigin.Current); // reserved
				newSibling.FrameCount = (ushort)(reader.ReadUInt16() >> 8);
				newSibling.CompressorName = Encoding.ASCII.GetString(reader.ReadBytes(32));
				newSibling.Depth = (ushort)(reader.ReadUInt16() >> 8);
				reader.BaseStream.Seek(2, SeekOrigin.Current); // pre_defined
				sibling.Children.AddRange(GetBoxes(reader, newSibling));
			}
		}

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
							where mdia_type.HandlerType == "soun"
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

				return;
			}
		}

		static Box CreateInstance(BinaryReader2 reader, out UInt32 size, out BoxType type)
		{
			size = reader.ReadUInt32();
			type = (BoxType)reader.ReadUInt32();
			Box box;

			switch (type)
			{
			case BoxType.Ftyp:
				box = new FileTypeBox();
				box.Size = size;
				box.Type = type;
				break;
			case BoxType.Moov:
				box = new MoovBox();
				box.Size = size;
				box.Type = type;
				break;
			case BoxType.Mvhd:
				{
					byte version = reader.ReadByte();
					byte[] bytes = reader.ReadBytes(3);
					UInt32 flags = (UInt32)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16));
					box = new MvhdBox(version, flags);
					box.Size = size;
					box.Type = type;
				}
				break;
			case BoxType.Trak:
				box = new TrakBox();
				box.Size = size;
				box.Type = type;
				break;
			case BoxType.Tkhd:
				box = null;
				break;
			default:
				box = null;
				break;
			}

			return box;
		}

		static void Main2(string[] args)
		{
			const string fileName = @"I:\Development\Data\Video\mp4_h264_aac.mp4";

			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var br = new BinaryReader2(fs, true))
			{
				UInt32 boxSize;
				BoxType boxType;
				Box box;

				while (fs.Position < fs.Length)
				{
					box = CreateInstance(br, out boxSize, out boxType);

					// ftyp
					if (boxType == BoxType.Ftyp)
					{
						FileTypeBox ftyp = box as FileTypeBox;
						ftyp.MajorBrand = br.ReadUInt32();
						ftyp.MinorVersion = br.ReadUInt32();
						ftyp.CompatibleBrands = br.ReadUInt32();

						Console.WriteLine("-----------------------------------------------------------------------------");
						Console.WriteLine(ftyp.GetName());
						Console.WriteLine("box size: {0:#,#}", ftyp.Size);
						Console.WriteLine(StringUtils.FromBinary(ftyp.MajorBrand));
						Console.WriteLine(StringUtils.FromBinary(ftyp.CompatibleBrands));
					}
					// moov
					else if (boxType == BoxType.Moov)
					{
						MoovBox moov = box as MoovBox;

						Console.WriteLine("-----------------------------------------------------------------------------");
						Console.WriteLine(moov.GetName());
						Console.WriteLine("box size: {0:#,#}", moov.Size);
					}
					// mvhd
					else if (boxType == BoxType.Mvhd)
					{
						MvhdBox mvhd = box as MvhdBox;

						if (mvhd.Version == 1)
						{
							mvhd.CreationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(br.ReadUInt64());
							mvhd.ModificationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(br.ReadUInt64());
							mvhd.TimeScale = br.ReadUInt32();
							mvhd.Duration = br.ReadUInt64();
						}
						else
						{
							mvhd.CreationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(br.ReadUInt32());
							mvhd.ModificationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(br.ReadUInt32());
							mvhd.TimeScale = br.ReadUInt32();
							mvhd.Duration = br.ReadUInt32();
						}
						mvhd.Rate = (Double)(br.ReadUInt32() >> 16);
						mvhd.Volume = (Single)(br.ReadInt16() >> 8);
						br.ReadBytes(2); // reserved1: bit(16)
						br.ReadBytes(4 * 2); // reserved2: unsigned int(32) [2]
						br.ReadBytes(4 * 9); // matrix: template int(32) [9]
						br.ReadBytes(4 * 6); // pre_defined: bit(32) [6]
						mvhd.NextTrackId = br.ReadUInt32();

						Console.WriteLine("-----------------------------------------------------------------------------");
						Console.WriteLine(mvhd.GetName());
						Console.WriteLine("box size: {0:#,#}", mvhd.Size);
						Console.WriteLine(mvhd.CreationTime);
						Console.WriteLine(mvhd.ModificationTime);
						Console.WriteLine(mvhd.TimeScale);
						Console.WriteLine(mvhd.Duration);
						Console.WriteLine(mvhd.Rate);
						Console.WriteLine(mvhd.Volume);
						Console.WriteLine(mvhd.NextTrackId);
					}
					// trak
					else if (boxType == BoxType.Trak)
					{
						TrakBox trak = box as TrakBox;

						Console.WriteLine("-----------------------------------------------------------------------------");
						Console.WriteLine(trak.GetName());
						Console.WriteLine("box size: {0:#,#}", trak.Size);
					}
					else
					{
						fs.Seek((boxSize - 4 * 2), SeekOrigin.Current);
						Console.WriteLine("-----------------------------------------------------------------------------");
						Console.WriteLine(StringUtils.FromBinary((uint)boxType));
						Console.WriteLine("box size: {0:#,#}", boxSize);
					}
				}
			}
		}
	}
}
