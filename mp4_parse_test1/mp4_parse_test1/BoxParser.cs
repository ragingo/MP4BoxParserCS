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
		private BinaryReader2 _reader;

		public BoxParser(BinaryReader2 reader)
		{
			_reader = reader;
		}

		public IEnumerable<Box> Parse()
		{
			return GetBoxes(null);
		}

		private List<Box> GetBoxes(Box parent)
		{
			var boxes = new List<Box>();

			while (_reader.BaseStream.Position < _reader.BaseStream.Length)
			{
				UInt32 boxSize = _reader.ReadUInt32();
				BoxType boxType = (BoxType)_reader.ReadUInt32();

				var sibling = BoxUtil.CreateInstance(boxType);
				sibling.Offset = _reader.BaseStream.Position - 4 * 2;
				sibling.Size = boxSize;
				sibling.Type = boxType;
				sibling.Parent = parent;
				boxes.Add(sibling);

				switch (boxType)
				{
				case BoxType.trak:
				case BoxType.mdia:
				case BoxType.dinf:
				case BoxType.udta:
					ParseDefaultParentBox(sibling);
					break;
				case BoxType.ftyp:
					ParseFtyp(sibling);
					break;
				case BoxType.moov:
					ParseMoov(sibling);
					break;
				case BoxType.mvhd:
					ParseMvhd(sibling);
					break;
				case BoxType.hdlr:
					ParseHdlr(sibling);
					break;
				case BoxType.minf:
					ParseMinf(sibling);
					break;
				case BoxType.dref:
					ParseDref(sibling);
					break;
				case BoxType.stbl:
					ParseStbl(sibling);
					break;
				case BoxType.stts:
					ParseStts(sibling);
					break;
				case BoxType.stsc:
					ParseStsc(sibling);
					break;
				case BoxType.stsz:
					ParseStsz(sibling);
					break;
				case BoxType.stsd:
					ParseStsd(sibling);
					break;
				case BoxType.avc1:
					ParseAvc1(sibling);
					break;
				case BoxType.mp4a:
					ParseMp4a(sibling);
					break;
				case BoxType.esds:
					ParseEsds(sibling);
					break;
				default:
					_reader.BaseStream.Seek((boxSize - 4 * 2), SeekOrigin.Current);
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

		private void ParseDefaultParentBox(Box sibling)
		{
			sibling.Children.AddRange(GetBoxes(sibling));
		}

		// File Type Box
		private void ParseFtyp(Box sibling)
		{
			var newSibling = sibling as FtypBox;
			newSibling.MajorBrand = _reader.ReadUInt32();
			newSibling.MinorVersion = _reader.ReadUInt32();

			while (newSibling.Offset + newSibling.Size != _reader.BaseStream.Position)
			{
				newSibling.CompatibleBrands.Add((Brand)_reader.ReadUInt32());
			}
		}

		// Movie Box
		private void ParseMoov(Box sibling)
		{
			sibling.Children.AddRange(GetBoxes(sibling));
		}

		// Movie Header Box
		private void ParseMvhd(Box sibling)
		{
			var newSibling = sibling as MvhdBox;
			newSibling.Version = _reader.ReadByte();
			newSibling.Flags = _reader.ReadUInt24();
			if (newSibling.Version == 1)
			{
				newSibling.CreationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(_reader.ReadUInt64());
				newSibling.ModificationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(_reader.ReadUInt64());
				newSibling.TimeScale = _reader.ReadUInt32();
				newSibling.Duration = _reader.ReadUInt64();
			}
			else
			{
				newSibling.CreationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(_reader.ReadUInt32());
				newSibling.ModificationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(_reader.ReadUInt32());
				newSibling.TimeScale = _reader.ReadUInt32();
				newSibling.Duration = _reader.ReadUInt32();
			}
			newSibling.Rate = (Double)(_reader.ReadUInt32() >> 16);
			newSibling.Volume = (Single)(_reader.ReadInt16() >> 8);
			_reader.ReadBytes(2); // reserved1: bit(16)
			_reader.ReadBytes(4 * 2); // reserved2: unsigned int(32) [2]
			_reader.ReadBytes(4 * 9); // matrix: template int(32) [9]
			_reader.ReadBytes(4 * 6); // pre_defined: bit(32) [6]
			newSibling.NextTrackId = _reader.ReadUInt32();
		}

		// Handler Reference Box
		private void ParseHdlr(Box sibling)
		{
			var newSibling = sibling as HdlrBox;
			newSibling.Version = _reader.ReadByte();
			newSibling.Flags = _reader.ReadUInt24();
			_reader.ReadUInt32(); // pre_defined: usigned int(32)
			newSibling.HandlerType = StringUtil.FromBinary(_reader.ReadUInt32());
			_reader.ReadBytes(4 * 3); // reserved: const unsigned int(32) [3]

			StringBuilder sb = new StringBuilder();
			char c;
			while((c = _reader.ReadChar()) != '\0')
			{
				sb.Append(c);
			}
			newSibling.Name = sb.ToString();
		}

		// Media Information Box
		private void ParseMinf(Box sibling)
		{
			sibling.Children.AddRange(GetBoxes(sibling));
		}

		// Data Reference Box
		private void ParseDref(Box sibling)
		{
			_reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			_reader.ReadUInt32();// entries
			sibling.Children.AddRange(GetBoxes(sibling));
		}

		// Sample Table Box
		private void ParseStbl(Box sibling)
		{
			sibling.Children.AddRange(GetBoxes(sibling));
		}

		// Decoding Time to Sample Box
		private void ParseStts(Box sibling)
		{
			var newSibling = sibling as SttsBox;
			_reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			newSibling.EntryCount = _reader.ReadUInt32();

			for (int i=0; i<newSibling.EntryCount; i++)
			{
				var entry = new SttsBox.Entry();
				entry.SampleCount = _reader.ReadUInt32();
				entry.SampleDelta = _reader.ReadUInt32();
				newSibling.Entries.Add(entry);
			}
		}

		// Sample To Chunk Box
		private void ParseStsc(Box sibling)
		{
			var newSibling = sibling as StscBox;
			_reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			newSibling.EntryCount = _reader.ReadUInt32();

			for (int i = 0; i < newSibling.EntryCount; i++)
			{
				var entry = new StscBox.Entry();
				entry.FirstChunk = _reader.ReadUInt32();
				entry.SamplesPerChunk = _reader.ReadUInt32();
				entry.SampleDescriptionIndex = _reader.ReadUInt32();
				newSibling.Entries.Add(entry);
			}
		}

		// Sample Size Box
		private void ParseStsz(Box sibling)
		{
			var newSibling = sibling as StszBox;
			_reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			newSibling.SampleSize = _reader.ReadUInt32();
			newSibling.SampleCount = _reader.ReadUInt32();

			if (newSibling.SampleSize == 0)
			{
				for (int i = 0; i < newSibling.SampleCount; i++)
				{
					var entry = new StszBox.Entry();
					entry.Size = _reader.ReadUInt32();
					newSibling.Entries.Add(entry);
				}
			}
		}

		// Sample Description Box
		private void ParseStsd(Box sibling)
		{
			_reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			var newSibling = sibling as StsdBox;
			newSibling.SampleEntries = _reader.ReadUInt32();
			newSibling.Children.AddRange(GetBoxes(newSibling));
		}

		// 8.5.1. p32 AudioSampleEntry
		private void ParseMp4a(Box sibling)
		{
			var newSibling = sibling as Mp4AudioSampleEntry;
			_reader.BaseStream.Seek(6 + 2, SeekOrigin.Current); // SampleEntry
			_reader.BaseStream.Seek(4 * 2, SeekOrigin.Current); // reserved
			newSibling.ChannelCount = _reader.ReadUInt16();
			newSibling.SampleSize = _reader.ReadUInt16();
			_reader.BaseStream.Seek(2 + 2, SeekOrigin.Current); // pre_defined + reserved
			newSibling.SampleRate = _reader.ReadUInt32() >> 16; // 16.16 hi.lo
			newSibling.Children.AddRange(GetBoxes(newSibling));
		}

		private void ParseEsds(Box sibling)
		{
			var newSibling = sibling as ESDescriptorBox;
			_reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
			newSibling.ES.Tag = (DescriptorTag)_reader.ReadByte();
			_reader.ReadBytes(3); // 0x80 * 3
			_reader.ReadBytes(1); // TOOD: TagSize・・・？ツールにはそう表示されていた・・・
			newSibling.ES.ESID = _reader.ReadUInt16();

			byte data1 = _reader.ReadByte();
			newSibling.ES.StreamDependenceFlag = (byte)((data1 & 0x80) >> 7);
			newSibling.ES.UrlFlag              = (byte)((data1 & 0x40) >> 6);
			newSibling.ES.OcrStreamFlag        = (byte)((data1 & 0x20) >> 5);
			newSibling.ES.StreamPriority       = (byte)((data1 & 0x1f) >> 0);

			// TODO: 動作未確認
			if (newSibling.ES.StreamDependenceFlag == 1)
			{
				newSibling.ES.DependsOnESID = _reader.ReadUInt16();
			}

			// TODO: 動作未確認
			if (newSibling.ES.UrlFlag == 1)
			{
				newSibling.ES.UrlLength = _reader.ReadByte();
				newSibling.ES.UrlString = Encoding.UTF8.GetString(_reader.ReadBytes(newSibling.ES.UrlLength));
			}

			// TODO: 動作未確認
			if (newSibling.ES.OcrStreamFlag == 1)
			{
				newSibling.ES.OcrESID = _reader.ReadUInt16();
			}

			newSibling.ES.DecConfigDescr.Tag = (DescriptorTag)_reader.ReadByte();
			_reader.ReadBytes(3); // 0x80 * 3
			_reader.ReadBytes(1); // TOOD: TagSize・・・？ツールにはそう表示されていた・・・
			newSibling.ES.DecConfigDescr.ObjectTypeIndication = (ObjectType)_reader.ReadByte();

			byte data2 = _reader.ReadByte();
			newSibling.ES.DecConfigDescr.StreamType = (byte)((data2 & 0xfc) >> 2);
			newSibling.ES.DecConfigDescr.UpStream   = (byte)((data2 & 0x02) >> 1);
			newSibling.ES.DecConfigDescr.Reserved   = (byte)((data2 & 0x01) >> 0);

			newSibling.ES.DecConfigDescr.BufferSizeDB = _reader.ReadUInt24();
			newSibling.ES.DecConfigDescr.MaxBitrate = _reader.ReadUInt32();
			newSibling.ES.DecConfigDescr.AvgBitrate = _reader.ReadUInt32();

			// TODO: "Audio Decoder Specific Info"(AudioSpecificConfig) の IF は ISO_IEC_14496-3 参照
			// TODO: まずは AudioSpecificConfig を定義する
			//newSibling.ES.DecConfigDescr.DecoderSpecificInfos[0] = new 

			// TODO: ...

			// TODO: 動作確認の為、BOX末尾へシーク
			_reader.BaseStream.Seek(sibling.Offset + sibling.Size, SeekOrigin.Begin);
		}

		// 8.5.1. p31 VisualSampleEntry
		private void ParseAvc1(Box sibling)
		{
			var newSibling = sibling as VisualSampleEntry;
			_reader.BaseStream.Seek(6, SeekOrigin.Current); // reserved
			newSibling.DataReferenceIndex = _reader.ReadUInt16();
			_reader.BaseStream.Seek(2 + 2 + 4 * 3, SeekOrigin.Current); // pre_defined + reserved + predefined
			newSibling.Width = _reader.ReadUInt16();
			newSibling.Height = _reader.ReadUInt16();
			newSibling.HorizontalResolution = _reader.ReadUInt32() >> 16;
			newSibling.VerticalResolution = _reader.ReadUInt32() >> 16;
			_reader.BaseStream.Seek(4, SeekOrigin.Current); // reserved
			newSibling.FrameCount = (ushort)(_reader.ReadUInt16() >> 8);

			byte[] data = _reader.ReadBytes(32).Reverse().Select(x => x == '\0' ? (byte)' ' : x).ToArray();
			newSibling.CompressorName = Encoding.ASCII.GetString(data).Trim();

			newSibling.Depth = (ushort)(_reader.ReadUInt16() >> 8);
			_reader.BaseStream.Seek(2, SeekOrigin.Current); // pre_defined
			newSibling.Children.AddRange(GetBoxes(newSibling));
		}
	}
}
