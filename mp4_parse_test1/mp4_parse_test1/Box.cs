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
		Url = ('u' << 24) | ('r' << 16) | ('l' << 8) | (' ' << 0),
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

	public class Box
	{
		public UInt32 Size { get; set; }
		public BoxType Type { get; set; }

		public Box(BoxType type)
		{
			Type = type;
		}

		public string GetName()
		{
			return StringUtils.FromBinary((UInt32)Type);
		}
	}

	public class FullBox : Box
	{
		public byte Version { get; set; }
		public UInt32 Flags { get; set; }

		public FullBox(BoxType type, byte version, UInt32 flags) : base(type)
		{
			Version = version;
			Flags = flags;
		}
	}

	public class FileTypeBox : Box
	{
		public UInt32 MajorBrand { get; set; }
		public UInt32 MinorVersion { get; set; }
		public UInt32 CompatibleBrands { get; set; }

		public FileTypeBox() : base(BoxType.Ftyp)
		{
		}
	}

	public class MoovBox : Box
	{
		public MoovBox() : base(BoxType.Moov)
		{
		}
	}

	public class MvhdBox : FullBox
	{
		public DateTime CreationTime { get; set; }
		public DateTime ModificationTime { get; set; }
		public UInt32 TimeScale { get; set; }
		public UInt64 Duration { get; set; }
		public Double Rate { get; set; }
		public Single Volume { get; set; }
		public UInt32 NextTrackId { get; set; }

		public MvhdBox(byte version, UInt32 flags) : base(BoxType.Mvhd, version, flags)
		{
		}
	}

	public class TrakBox : Box
	{
		public TrakBox() : base(BoxType.Trak)
		{
		}
	}

	public class TkhdBox : FullBox
	{
		public TkhdBox(byte version, UInt32 flags) : base(BoxType.Tkhd, version, flags)
		{
		}
	}
}
