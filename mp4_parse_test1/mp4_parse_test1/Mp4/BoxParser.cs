﻿using System;
using System.IO;
using System.Text;
using mp4_parse_test1.Boxes;
using mp4_parse_test1.Descriptors;

namespace mp4_parse_test1
{
    class RootBox : Box
    {
        public RootBox() : base(BoxType.Unknown)
        {
        }
    }

    class BoxParser
    {
        private BinaryReader _reader;
        private Mp4Container _container;

        public BoxParser(BinaryReader reader, Mp4Container container)
        {
            _reader = reader;
            _container = container;
        }

        public void Parse()
        {
            var root = new RootBox();

            ParseBoxes(root);

            _container.Boxes.AddRange(root.Children);
        }

        private void ParseBoxes(Box parent)
        {
            while (_reader.BaseStream.Position < _reader.BaseStream.Length)
            {
                UInt32 boxSize = _reader.ReadUInt32();
                BoxType boxType = (BoxType)_reader.ReadUInt32();

                var sibling = BoxUtil.CreateInstance(boxType);
                sibling.Parent = parent;
                parent.Children.Add(sibling);

                ParseBox(sibling, boxSize);

                switch (boxType)
                {
                    case BoxType.moov:
                    case BoxType.trak:
                    case BoxType.mdia:
                    case BoxType.minf:
                    case BoxType.dinf:
                    case BoxType.udta:
                        ParseDefaultParentBox(sibling);
                        break;
                    case BoxType.ftyp:
                        ParseFtyp(sibling);
                        break;
                    case BoxType.mdat:
                        ParseMdat(sibling);
                        break;
                    case BoxType.mvhd:
                        ParseMvhd(sibling);
                        break;
                    case BoxType.hdlr:
                        ParseHdlr(sibling);
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
                    case BoxType.stco:
                        ParseStco(sibling);
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

                if (sibling.IsLastPosition())
                {
                    return;
                }
            }
        }

        private void ParseBox(Box sibling, uint boxSize)
        {
            sibling.Size = boxSize;
            sibling.Offset = _reader.BaseStream.Position - 4 * 2;

            if (sibling.Size == 1)
            {
                sibling.LargeSize = _reader.ReadUInt64();
            }
        }

        private void ParseDefaultParentBox(Box sibling)
        {
            ParseBoxes(sibling);
        }

        // File Type Box
        private void ParseFtyp(Box sibling)
        {
            var box = sibling as FileTypeBox;
            box.MajorBrand = _reader.ReadUInt32();
            box.MinorVersion = _reader.ReadUInt32();

            while (box.Offset + box.Size != _reader.BaseStream.Position)
            {
                box.CompatibleBrands.Add((Brand)_reader.ReadUInt32());
            }
        }

        // Media Data Box
        private void ParseMdat(Box sibling)
        {
            var box = sibling as MediaDataBox;
            // TODO: これは重たすぎるから改善する
            box.Data = _reader.ReadBytes((int)box.Size - (4 + 4)); // size + type
        }

        // Movie Header Box
        private void ParseMvhd(Box sibling)
        {
            var box = sibling as MovieHeaderBox;
            box.Version = _reader.ReadByte();
            box.Flags = _reader.ReadUInt24();
            if (box.Version == 1)
            {
                box.CreationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(_reader.ReadUInt64());
                box.ModificationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(_reader.ReadUInt64());
                box.TimeScale = _reader.ReadUInt32();
                box.Duration = _reader.ReadUInt64();
            }
            else
            {
                box.CreationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(_reader.ReadUInt32());
                box.ModificationTime = (new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(_reader.ReadUInt32());
                box.TimeScale = _reader.ReadUInt32();
                box.Duration = _reader.ReadUInt32();
            }
            box.Rate = (Double)(_reader.ReadUInt32() >> 16);
            box.Volume = (Single)(_reader.ReadInt16() >> 8);
            box.Reserved = _reader.ReadUInt16();
            Array.Copy(_reader.ReadBytes(4 * 2), box.Reserved2, box.Reserved2.Length);
            Array.Copy(_reader.ReadBytes(4 * 9), box.Matrix, box.Matrix.Length);
            Array.Copy(_reader.ReadBytes(4 * 6), box.PreDefined, box.PreDefined.Length);
            box.NextTrackId = _reader.ReadUInt32();
        }

        // Handler Reference Box
        private void ParseHdlr(Box sibling)
        {
            var box = sibling as HandlerBox;
            box.Version = _reader.ReadByte();
            box.Flags = _reader.ReadUInt24();
            box.PreDefined = _reader.ReadUInt32();
            box.HandlerType = (HandlerType)_reader.ReadUInt32();
            Array.Copy(_reader.ReadBytes(4 * 3), box.Reserved, box.Reserved.Length);

            StringBuilder sb = new StringBuilder();
            char c;
            while ((c = _reader.ReadChar()) != '\0')
            {
                sb.Append(c);
            }
            box.Name = sb.ToString();
        }

        // Data Reference Box
        private void ParseDref(Box sibling)
        {
            _reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
            _reader.ReadUInt32();// entries
            ParseBoxes(sibling);
        }

        // Sample Table Box
        private void ParseStbl(Box sibling)
        {
            ParseBoxes(sibling);
        }

        // Decoding Time to Sample Box
        private void ParseStts(Box sibling)
        {
            var box = sibling as TimeToSampleBox;
            _reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
            box.EntryCount = _reader.ReadUInt32();

            for (int i = 0; i < box.EntryCount; i++)
            {
                var entry = new TimeToSampleBox.Entry();
                entry.SampleCount = _reader.ReadUInt32();
                entry.SampleDelta = _reader.ReadUInt32();
                box.Entries.Add(entry);
            }
        }

        // Sample To Chunk Box
        private void ParseStsc(Box sibling)
        {
            var box = sibling as SampleToChunkBox;
            _reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
            box.EntryCount = _reader.ReadUInt32();

            for (int i = 0; i < box.EntryCount; i++)
            {
                var entry = new SampleToChunkBox.Entry();
                entry.FirstChunk = _reader.ReadUInt32();
                entry.SamplesPerChunk = _reader.ReadUInt32();
                entry.SampleDescriptionIndex = _reader.ReadUInt32();
                box.Entries.Add(entry);
            }
        }

        // Sample Size Box
        private void ParseStsz(Box sibling)
        {
            var box = sibling as SampleSizeBox;
            _reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
            box.SampleSize = _reader.ReadUInt32();
            box.SampleCount = _reader.ReadUInt32();

            if (box.SampleSize == 0)
            {
                for (int i = 0; i < box.SampleCount; i++)
                {
                    var entry = new SampleSizeBox.Entry();
                    entry.Size = _reader.ReadUInt32();
                    box.Entries.Add(entry);
                }
            }
        }

        // Chunk Offset Box
        private void ParseStco(Box sibling)
        {
            var box = sibling as ChunkOffsetBox;
            _reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
            box.EntryCount = _reader.ReadUInt32();

            for (int i = 0; i < box.EntryCount; i++)
            {
                var entry = new ChunkOffsetBox.Entry();
                entry.ChunkOffset = _reader.ReadUInt32();
                box.Entries.Add(entry);
            }
        }

        // Sample Description Box
        private void ParseStsd(Box sibling)
        {
            _reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
            var box = sibling as SampleDescriptionBox;
            box.EntryCount = _reader.ReadUInt32();
            ParseBoxes(box);

            //for (int i = 0; i < box.EntryCount; i++)
            //{
            //	var entry = new SampleDescriptionBox.Entry();
            //	var hdlr = box.Parent.Parent.Parent.GetChild<HandlerBox>();

            //	switch (hdlr.HandlerType)
            //	{
            //	case HandlerType.Video:
            //		entry.SampleEntry = new Mp4VisualSampleEntry();
            //		ParseAvc1(box); // TODO: メソッド名(avc1)は適切かどうか？
            //		break;
            //	case HandlerType.Sound:
            //		entry.SampleEntry = new Mp4AudioSampleEntry();
            //		ParseMp4a(box);
            //		break;
            //	//case HandlerType.Hint:
            //	//	entry = new HintSampleEntry();
            //	//	break;
            //	//case HandlerType.Metadata:
            //	//	entry = new MetaDataSampleEntry();
            //	//	break;
            //	default:
            //		break;
            //	}

            //	box.Entries.Add(entry);
            //}
        }

        // 8.5.1. p32 AudioSampleEntry
        private void ParseMp4a(Box sibling)
        {
            var box = sibling as Mp4AudioSampleEntry;
            _reader.BaseStream.Seek(6 + 2, SeekOrigin.Current); // SampleEntry
            _reader.BaseStream.Seek(4 * 2, SeekOrigin.Current); // reserved
            box.ChannelCount = _reader.ReadUInt16();
            box.SampleSize = _reader.ReadUInt16();
            _reader.BaseStream.Seek(2 + 2, SeekOrigin.Current); // pre_defined + reserved
            box.SampleRate = _reader.ReadUInt32() >> 16; // 16.16 hi.lo
            ParseBoxes(box);
        }

        private void ParseEsds(Box sibling)
        {
            var box = sibling as EsdBox;
            _reader.BaseStream.Seek(4, SeekOrigin.Current); // FullBox
            box.ES.Tag = (DescriptorTag)_reader.ReadByte();
            _reader.ReadBytes(3); // 0x80 * 3
            _reader.ReadBytes(1); // TOOD: TagSize・・・？ツールにはそう表示されていた・・・
            box.ES.ESID = _reader.ReadUInt16();

            byte data1 = _reader.ReadByte();
            box.ES.StreamDependenceFlag = (byte)((data1 & 0x80) >> 7);
            box.ES.UrlFlag = (byte)((data1 & 0x40) >> 6);
            box.ES.OcrStreamFlag = (byte)((data1 & 0x20) >> 5);
            box.ES.StreamPriority = (byte)((data1 & 0x1f) >> 0);

            // TODO: 動作未確認
            if (box.ES.StreamDependenceFlag == 1)
            {
                box.ES.DependsOnESID = _reader.ReadUInt16();
            }

            // TODO: 動作未確認
            if (box.ES.UrlFlag == 1)
            {
                box.ES.UrlLength = _reader.ReadByte();
                box.ES.UrlString = StringUtil.FromBinary(_reader.ReadBytes(box.ES.UrlLength), Encoding.UTF8);
            }

            // TODO: 動作未確認
            if (box.ES.OcrStreamFlag == 1)
            {
                box.ES.OcrESID = _reader.ReadUInt16();
            }

            box.ES.DecConfigDescr.Tag = (DescriptorTag)_reader.ReadByte();
            _reader.ReadBytes(3); // 0x80 * 3
            _reader.ReadBytes(1); // TOOD: TagSize・・・？ツールにはそう表示されていた・・・
            box.ES.DecConfigDescr.ObjectTypeIndication = (ObjectType)_reader.ReadByte();

            byte data2 = _reader.ReadByte();
            box.ES.DecConfigDescr.StreamType = (byte)((data2 & 0xfc) >> 2);
            box.ES.DecConfigDescr.UpStream = (byte)((data2 & 0x02) >> 1);
            box.ES.DecConfigDescr.Reserved = (byte)((data2 & 0x01) >> 0);

            box.ES.DecConfigDescr.BufferSizeDB = _reader.ReadUInt24();
            box.ES.DecConfigDescr.MaxBitrate = _reader.ReadUInt32();
            box.ES.DecConfigDescr.AvgBitrate = _reader.ReadUInt32();

            var descriptor = DescriptorUtil.CreateInstance(box.ES.DecConfigDescr.ObjectTypeIndication);
            box.ES.DecConfigDescr.DecoderSpecificInfos[0] = descriptor;

            descriptor.Tag = (DescriptorTag)_reader.ReadByte();
            _reader.ReadBytes(3); // 0x80 * 3
            _reader.ReadBytes(1); // TOOD: TagSize・・・？ツールにはそう表示されていた・・・

            // TODO: ...
            if (descriptor is AudioSpecificConfig)
            {
                var audio = descriptor as AudioSpecificConfig;

                byte[] bits = _reader.ReadBytes(10); // TODO: 5bits + 4bits とかで読み難い・・・
                audio.AudioObjectType = (AudioObjectType)((bits[0] & 0xf8) >> 3);
                audio.SamplingFrequencyIndex = (byte)(((bits[0] & 0x07) << 1) | ((bits[1] & 0x80) >> 7)); // TODO: バグ
            }

            // TODO: 動作確認の為、BOX末尾へシーク
            _reader.BaseStream.Seek(sibling.Offset + sibling.Size, SeekOrigin.Begin);
        }

        private void ParseAvc1(Box sibling)
        {
            var box = sibling as VisualSampleEntry;
            _reader.BaseStream.Seek(6, SeekOrigin.Current); // reserved
            box.DataReferenceIndex = _reader.ReadUInt16();
            _reader.BaseStream.Seek(2 + 2 + 4 * 3, SeekOrigin.Current); // pre_defined + reserved + predefined
            box.Width = _reader.ReadUInt16();
            box.Height = _reader.ReadUInt16();
            box.HorizontalResolution = _reader.ReadUInt32() >> 16;
            box.VerticalResolution = _reader.ReadUInt32() >> 16;
            _reader.BaseStream.Seek(4, SeekOrigin.Current); // reserved
            box.FrameCount = (ushort)(_reader.ReadUInt16() >> 8);
            box.CompressorName = StringUtil.FromBinary(_reader.ReadBytes(32)).Trim();
            box.Depth = (ushort)(_reader.ReadUInt16() >> 8);
            _reader.BaseStream.Seek(2, SeekOrigin.Current); // pre_defined
            ParseBoxes(box);
        }
    }
}
