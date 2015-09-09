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
		public List<Box> Parse(BinaryReader2 reader)
		{
			return GetBoxes(reader, null);
		}

		private List<Box> GetBoxes(BinaryReader2 reader, Box parent)
		{
			var boxes = new List<Box>();

			while (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				UInt32 boxSize = reader.ReadUInt32();
				BoxType boxType = (BoxType)reader.ReadUInt32();

				var sibling = new Box();
				sibling.Offset = reader.BaseStream.Position - 4 * 2;
				sibling.Size = boxSize;
				sibling.Type = boxType;
				sibling.Parent = parent;

				switch (boxType)
				{
				case BoxType.trak:
				case BoxType.mdia:
				case BoxType.dinf:
				case BoxType.udta:
					boxes.Add(sibling);
					sibling.Children.AddRange(GetBoxes(reader, sibling));
					break;
				case BoxType.ftyp:
					boxes.Add(ParseFtyp(reader, sibling));
					break;
				case BoxType.moov:
					boxes.Add(ParseMoov(reader, sibling));
					break;
				case BoxType.mvhd:
					boxes.Add(ParseMvhd(reader, sibling));
					break;
				case BoxType.hdlr:
					boxes.Add(ParseHdlr(reader, sibling));
					break;
				case BoxType.minf:
					boxes.Add(ParseMinf(reader, sibling));
					break;
				case BoxType.dref:
					boxes.Add(ParseDref(reader, sibling));
					break;
				case BoxType.stbl:
					boxes.Add(ParseStbl(reader, sibling));
					break;
				case BoxType.stts:
					boxes.Add(ParseStts(reader, sibling));
					break;
				case BoxType.stsc:
					boxes.Add(ParseStsc(reader, sibling));
					break;
				case BoxType.stsz:
					boxes.Add(ParseStsz(reader, sibling));
					break;
				case BoxType.stsd:
					boxes.Add(ParseStsd(reader, sibling));
					break;
				case BoxType.avc1:
					boxes.Add(ParseAvc1(reader, sibling));
					break;
				case BoxType.mp4a:
					boxes.Add(ParseMp4a(reader, sibling));
					break;
				case BoxType.esds:
					boxes.Add(ParseEsds(reader, sibling));
					break;
				default:
					boxes.Add(sibling);
					reader.BaseStream.Seek((boxSize - 4 * 2), SeekOrigin.Current);
					break;
				}

				if (parent != null &&
					parent.Offset + parent.Size == sibling.Offset + sibling.Size)
				{
					return boxes;
				}
			}

			return boxes;
		}

		// File Type Box
		private Box ParseFtyp(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<FtypBox>();
			newSibling.MajorBrand = reader.ReadUInt32();
			newSibling.MinorVersion = reader.ReadUInt32();

			while (newSibling.Offset + newSibling.Size != reader.BaseStream.Position)
			{
				newSibling.CompatibleBrands.Add((Brand)reader.ReadUInt32());
			}

			return newSibling;
		}

		// Movie Box
		private Box ParseMoov(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<MoovBox>();
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));
			return newSibling;
		}

		// Movie Header Box
		private Box ParseMvhd(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<MvhdBox>();
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
		private Box ParseHdlr(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<HdlrBox>();
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

		// Media Information Box
		private Box ParseMinf(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<MinfBox>();
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));
			return newSibling;
		}

		// Data Reference Box
		private Box ParseDref(BinaryReader2 reader, Box sibling)
		{
			reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			reader.ReadUInt32();// entries
			sibling.Children.AddRange(GetBoxes(reader, sibling));

			return sibling;
		}

		// Sample Table Box
		private Box ParseStbl(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<StblBox>();
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));
			return newSibling;
		}

		// Decoding Time to Sample Box
		private Box ParseStts(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<SttsBox>();
			reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			newSibling.EntryCount = reader.ReadUInt32();

			for (int i=0; i<newSibling.EntryCount; i++)
			{
				var entry = new SttsBox.Entry();
				entry.SampleCount = reader.ReadUInt32();
				entry.SampleDelta = reader.ReadUInt32();
				newSibling.Entries.Add(entry);
			}
			return newSibling;
		}

		// Sample To Chunk Box
		private Box ParseStsc(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<StscBox>();
			reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			newSibling.EntryCount = reader.ReadUInt32();

			for (int i = 0; i < newSibling.EntryCount; i++)
			{
				var entry = new StscBox.Entry();
				entry.FirstChunk = reader.ReadUInt32();
				entry.SamplesPerChunk = reader.ReadUInt32();
				entry.SampleDescriptionIndex = reader.ReadUInt32();
				newSibling.Entries.Add(entry);
			}
			return newSibling;
		}

		// Sample Size Box
		private Box ParseStsz(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<StszBox>();
			reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			newSibling.SampleSize = reader.ReadUInt32();
			newSibling.SampleCount = reader.ReadUInt32();

			if (newSibling.SampleSize == 0)
			{
				for (int i = 0; i < newSibling.SampleCount; i++)
				{
					var entry = new StszBox.Entry();
					entry.Size = reader.ReadUInt32();
					newSibling.Entries.Add(entry);
				}
			}

			return newSibling;
		}

		// Sample Description Box
		private Box ParseStsd(BinaryReader2 reader, Box sibling)
		{
			reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			var newSibling = sibling.As<StsdBox>();
			newSibling.SampleEntries = reader.ReadUInt32();
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));

			return newSibling;
		}

		// 8.5.1. p32 AudioSampleEntry
		private Box ParseMp4a(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<Mp4AudioSampleEntry>();
			reader.BaseStream.Seek(6 + 2, SeekOrigin.Current); // SampleEntry
			reader.BaseStream.Seek(4 * 2, SeekOrigin.Current); // reserved
			newSibling.ChannelCount = reader.ReadUInt16();
			newSibling.SampleSize = reader.ReadUInt16();
			reader.BaseStream.Seek(2 + 2, SeekOrigin.Current); // pre_defined + reserved
			newSibling.SampleRate = reader.ReadUInt32() >> 16; // 16.16 hi.lo
			newSibling.Children.AddRange(GetBoxes(reader, newSibling));

			return newSibling;
		}

		private Box ParseEsds(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<ESDescriptorBox>();
			reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			newSibling.ES.Tag = (DescriptorTag)reader.ReadByte();
			reader.ReadBytes(3); // 0x80 * 3
			reader.ReadBytes(1); // TOOD: TagSize・・・？ツールにはそう表示されていた・・・
			newSibling.ES.ESID = reader.ReadUInt16();

			byte data1 = reader.ReadByte();
			newSibling.ES.StreamDependenceFlag = (byte)((data1 & 0x80) >> 7);
			newSibling.ES.UrlFlag              = (byte)((data1 & 0x40) >> 6);
			newSibling.ES.OcrStreamFlag        = (byte)((data1 & 0x20) >> 5);
			newSibling.ES.StreamPriority       = (byte)((data1 & 0x1f) >> 0);

			// TODO: 動作未確認
			if (newSibling.ES.StreamDependenceFlag == 1)
			{
				newSibling.ES.DependsOnESID = reader.ReadUInt16();
			}

			// TODO: 動作未確認
			if (newSibling.ES.UrlFlag == 1)
			{
				newSibling.ES.UrlLength = reader.ReadByte();
				newSibling.ES.UrlString = Encoding.UTF8.GetString(reader.ReadBytes(newSibling.ES.UrlLength));
			}

			// TODO: 動作未確認
			if (newSibling.ES.OcrStreamFlag == 1)
			{
				newSibling.ES.OcrESID = reader.ReadUInt16();
			}

			newSibling.ES.DecConfigDescr.Tag = (DescriptorTag)reader.ReadByte();
			reader.ReadBytes(3); // 0x80 * 3
			reader.ReadBytes(1); // TOOD: TagSize・・・？ツールにはそう表示されていた・・・
			newSibling.ES.DecConfigDescr.ObjectTypeIndication = (ObjectType)reader.ReadByte();

			byte data2 = reader.ReadByte();
			newSibling.ES.DecConfigDescr.StreamType = (byte)((data2 & 0xfc) >> 2);
			newSibling.ES.DecConfigDescr.UpStream   = (byte)((data2 & 0x02) >> 1);
			newSibling.ES.DecConfigDescr.Reserved   = (byte)((data2 & 0x01) >> 0);

			newSibling.ES.DecConfigDescr.BufferSizeDB = reader.ReadUInt24();
			newSibling.ES.DecConfigDescr.MaxBitrate = reader.ReadUInt32();
			newSibling.ES.DecConfigDescr.AvgBitrate = reader.ReadUInt32();

			// TODO: "Audio Decoder Specific Info"(AudioSpecificConfig) の IF は ISO_IEC_14496-3 参照
			// TODO: まずは AudioSpecificConfig を定義する
			//newSibling.ES.DecConfigDescr.DecoderSpecificInfos[0] = new 

			// TODO: ...

			// TODO: 動作確認の為、BOX末尾へシーク
			reader.BaseStream.Seek(sibling.Offset + sibling.Size, SeekOrigin.Begin);

			return newSibling;
		}

		// 8.5.1. p31 VisualSampleEntry
		private Box ParseAvc1(BinaryReader2 reader, Box sibling)
		{
			var newSibling = sibling.As<VisualSampleEntry>();
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
