using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1
{
	class BoxParser
	{
		public List<BoxNode> GetBoxes(BinaryReader2 reader)
		{
			return GetBoxes(reader, null);
		}

		private List<BoxNode> GetBoxes(BinaryReader2 reader, BoxNode parent)
		{
			var nodes = new List<BoxNode>();

			while (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				UInt32 boxSize = reader.ReadUInt32();
				BoxType boxType = (BoxType)reader.ReadUInt32();

				var sibling = new BoxNode();
				sibling.Offset = reader.BaseStream.Position - 4 * 2;
				sibling.Size = boxSize;
				sibling.Type = boxType;
				sibling.Parent = parent;

				switch (boxType)
				{
				case BoxType.trak:
				case BoxType.mdia:
				case BoxType.minf:
				case BoxType.dinf:
				case BoxType.stbl:
				case BoxType.udta:
					nodes.Add(sibling);
					sibling.Children.AddRange(GetBoxes(reader, sibling));
					break;
				case BoxType.moov:
					nodes.Add(ParseMoov(reader, sibling));
					break;
				case BoxType.mvhd:
					nodes.Add(ParseMvhd(reader, sibling));
					break;
				case BoxType.hdlr:
					nodes.Add(ParseHdlr(reader, sibling));
					break;
				case BoxType.dref:
					nodes.Add(ParseDref(reader, sibling));
					break;
				case BoxType.stsd:
					nodes.Add(ParseStsd(reader, sibling));
					break;
				case BoxType.avc1:
					nodes.Add(ParseAvc1(reader, sibling));
					break;
				case BoxType.mp4a:
					nodes.Add(ParseMp4a(reader, sibling));
					break;
				default:
					nodes.Add(sibling);
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

		private BoxNode ParseMoov(BinaryReader2 reader, BoxNode sibling)
		{
			var newSibling = sibling.As<MoovBoxNode>();
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));
			return newSibling;
		}

		// Movie Header Box
		private BoxNode ParseMvhd(BinaryReader2 reader, BoxNode sibling)
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

			return newSibling;
		}

		// Handler Reference Box
		private BoxNode ParseHdlr(BinaryReader2 reader, BoxNode sibling)
		{
			var newSibling = sibling.As<HdlrBoxNode>();
			newSibling.Version = reader.ReadByte();
			newSibling.Flags = reader.ReadUInt24();
			reader.ReadUInt32(); // pre_defined: usigned int(32)
			newSibling.HandlerType = StringUtils.FromBinary(reader.ReadUInt32());
			reader.ReadBytes(4 * 3); // reserved: const unsigned int(32) [3]

			StringBuilder sb = new StringBuilder();
			char c;
			while((c = reader.ReadChar()) != '\0')
			{
				sb.Append(c);
			}
			newSibling.Name = sb.ToString();

			return newSibling;
		}

		// Data Reference Box
		private BoxNode ParseDref(BinaryReader2 reader, BoxNode sibling)
		{
			reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			reader.ReadUInt32();// entries
			sibling.Children.AddRange(GetBoxes(reader, sibling));

			return sibling;
		}

		// Sample Description Box
		private BoxNode ParseStsd(BinaryReader2 reader, BoxNode sibling)
		{
			reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			var newSibling = sibling.As<StsdBoxNode>();
			newSibling.SampleEntries = reader.ReadUInt32();
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));

			return newSibling;
		}

		// 8.5.1. p32 AudioSampleEntry
		private BoxNode ParseMp4a(BinaryReader2 reader, BoxNode sibling)
		{
			var newSibling = sibling.As<AudioSampleEntryNode>();
			reader.BaseStream.Seek(6 + 2, SeekOrigin.Current); // SampleEntry
			reader.BaseStream.Seek(4 * 2, SeekOrigin.Current); // reserved
			newSibling.ChannelCount = reader.ReadUInt16();
			newSibling.SampleSize = reader.ReadUInt16();
			reader.BaseStream.Seek(2 + 2, SeekOrigin.Current); // pre_defined + reserved
			newSibling.SampleRate = reader.ReadUInt32();
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));

			return newSibling;
		}

		// 8.5.1. p31 VisualSampleEntry
		private BoxNode ParseAvc1(BinaryReader2 reader, BoxNode sibling)
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
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));

			return newSibling;
		}
	}
}
