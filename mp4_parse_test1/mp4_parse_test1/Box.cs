using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1
{
	public enum BoxType : uint
	{
		Unknown = uint.MaxValue,
		Ftyp = ('f' << 24) | ('t' << 16) | ('y' << 8) | ('p' << 0),
		Moov = ('m' << 24) | ('o' << 16) | ('o' << 8) | ('v' << 0),
		Mvhd = ('m' << 24) | ('v' << 16) | ('h' << 8) | ('d' << 0),
		Iods = ('i' << 24) | ('o' << 16) | ('d' << 8) | ('s' << 0),
		Trak = ('t' << 24) | ('r' << 16) | ('a' << 8) | ('k' << 0),
		Tkhd = ('t' << 24) | ('k' << 16) | ('h' << 8) | ('d' << 0),
		Mdia = ('m' << 24) | ('d' << 16) | ('i' << 8) | ('a' << 0),
		Mdhd = ('m' << 24) | ('d' << 16) | ('h' << 8) | ('d' << 0),
		Hdlr = ('h' << 24) | ('d' << 16) | ('l' << 8) | ('r' << 0),
		Minf = ('m' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		Vmhd = ('v' << 24) | ('m' << 16) | ('h' << 8) | ('d' << 0),
		Dinf = ('d' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		Dref = ('d' << 24) | ('r' << 16) | ('e' << 8) | ('f' << 0),
		Url  = ('u' << 24) | ('r' << 16) | ('l' << 8) | (' ' << 0),
		Stbl = ('s' << 24) | ('t' << 16) | ('b' << 8) | ('l' << 0),
		Smhd = ('s' << 24) | ('m' << 16) | ('h' << 8) | ('d' << 0),
		Stsd = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('d' << 0),
		Avc1 = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('1' << 0),
		AvcC = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('C' << 0),
		Btrt = ('b' << 24) | ('t' << 16) | ('r' << 8) | ('t' << 0),
		Mp4a = ('m' << 24) | ('p' << 16) | ('4' << 8) | ('a' << 0),
		Esds = ('e' << 24) | ('s' << 16) | ('d' << 8) | ('s' << 0),
		Stts = ('s' << 24) | ('t' << 16) | ('t' << 8) | ('s' << 0),
		Stss = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('s' << 0),
		Stsc = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('c' << 0),
		Stsz = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('z' << 0),
		Stco = ('s' << 24) | ('t' << 16) | ('c' << 8) | ('o' << 0),
		Udta = ('u' << 24) | ('d' << 16) | ('t' << 8) | ('a' << 0),
		Cprt = ('c' << 24) | ('p' << 16) | ('r' << 8) | ('t' << 0),
		Mdat = ('m' << 24) | ('d' << 16) | ('a' << 8) | ('t' << 0),
		Free = ('f' << 24) | ('r' << 16) | ('e' << 8) | ('e' << 0),
	}

	public static class HandlerTypes
	{
		public const string Sound = "soun";
		public const string Video = "vide";
		public const string Hint = "hint";
		public const string Metadata = "meta";
		public const string AuxiliaryVideo = "auxv";
	}

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

	class MoovBoxNode : BoxNode
	{
		public MoovBoxNode()
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

	class HdlrBoxNode : FullBoxNode
	{
		public string HandlerType { get; set; }
		public string Name { get; set; }

		public HdlrBoxNode()
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

	class AudioSampleEntryNode : SampleEntryNode
	{
		public new UInt32[] Reserved { get; set; }
		public UInt16 ChannelCount { get; set; }
		public UInt16 SampleSize { get; set; }
		public UInt16 PreDefined { get; set; }
		public UInt16 Reserved2 { get; set; }
		public UInt32 SampleRate { get; set; }

		public AudioSampleEntryNode()
		{
			Reserved = new UInt32[2];
			ChannelCount = 2;
			SampleSize = 16;
		}
	}

	// http://xhelmboyx.tripod.com/formats/mp4-layout.txt
	// TODO: esds box. 複数のタグが管理されるみたい・・・どう扱うのか・・・
	class ESDescriptorBoxNode : FullBoxNode
	{
		public byte Tag { get; set; }
		public byte TagSize { get; set; }
		public UInt16 ESID { get; set; }
		public byte StreamPriority { get; set; }
	}
}
