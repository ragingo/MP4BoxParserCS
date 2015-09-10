using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mp4_parse_test1.Boxes;

namespace mp4_parse_test1
{

	public abstract class BoxBase
	{
		public UInt32 Size { get; set; }
		public BoxType Type { get { return _type; } }

		private BoxType _type;

		public BoxBase(BoxType type)
		{
			_type = type;
		}
	}

	/// <summary>
	/// ISO/IEC 14496-12:2012(E) Box
	/// </summary>
	public class Box : BoxBase
	{
		public UInt64 LargeSize { get; set; }

		public long Offset { get; set; }
		public Box Parent { get; set; }
		public List<Box> Children { get; private set; }

		public Box(BoxType type) : base(type)
		{
			Children = new List<Box>();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(StringUtil.FromBinary((uint)Type));
			sb.AppendFormat(" {{ offset:{0:#,0}, size:{1:#,0} }}", Offset, Size);
			return sb.ToString();
		}

		public T GetChild<T>()
			where T : Box, new()
		{
			return Children.FirstOrDefault(b => b.GetType() == typeof(T)) as T;
		}

		public Box GetChild(BoxType type)
		{
			return Children.FirstOrDefault(b => b.Type == type);
		}

		public bool HasChild()
		{
			return Children.Count > 0;
		}
	}


	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 
	/// </summary>
	public class SttsBox : FullBox
	{
		public UInt32 EntryCount { get; set; }

		public class Entry
		{
			public UInt32 SampleCount { get; set; }
			public UInt32 SampleDelta { get; set; }
		}
		public List<Entry> Entries { get; private set; }

		public SttsBox() : base(BoxType.stts)
		{
			Entries = new List<Entry>();
		}
	}

	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 
	/// </summary>
	public class StscBox : FullBox
	{
		public UInt32 EntryCount { get; set; }

		public class Entry
		{
			public UInt32 FirstChunk { get; set; }
			public UInt32 SamplesPerChunk { get; set; }
			public UInt32 SampleDescriptionIndex { get; set; }
		}
		public List<Entry> Entries { get; private set; }

		public StscBox() : base(BoxType.stsc)
		{
			Entries = new List<Entry>();
		}
	}

	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 
	/// </summary>
	public class StszBox : FullBox
	{
		public UInt32 SampleSize { get; set; }
		public UInt32 SampleCount { get; set; }

		public class Entry
		{
			public UInt32 Size { get; set; }
		}
		public List<Entry> Entries { get; private set; }

		public StszBox() : base(BoxType.stsz)
		{
			Entries = new List<Entry>();
		}
	}

	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 
	/// </summary>
	public class StcoBox : FullBox
	{
		public UInt32 EntryCount { get; set; }

		public class Entry
		{
			public UInt32 ChunkOffset { get; set; }
		}
		public List<Entry> Entries { get; private set; }

		public StcoBox() : base(BoxType.stco)
		{
			Entries = new List<Entry>();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class ESDescriptorBox : FullBox
	{
		public ESDescriptor ES { get; set; }

		public ESDescriptorBox() : base(BoxType.esds)
		{
			ES = new ESDescriptor();
		}
	}

	//// mp4v
	//public class Mp4VisualSampleEntry : VisualSampleEntry
	//{
	//	public Mp4VisualSampleEntry()
	//	{
	//	}
	//}

	// mp4a
	public class Mp4AudioSampleEntry : AudioSampleEntry
	{
		public Mp4AudioSampleEntry() : base(BoxType.mp4a)
		{
		}
	}

	//// mp4s
	//public class MpegSampleEntry : SampleEntry
	//{
	//	public MpegSampleEntry()
	//	{
	//	}
	//}
}
