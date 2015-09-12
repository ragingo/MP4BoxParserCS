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

	// TODO: 次に定義するのは、8.7.6 Padding Bits Box

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

	// mp4v
	public class Mp4VisualSampleEntry : VisualSampleEntry
	{
		public Mp4VisualSampleEntry() : base(SampleEntryCode.mp4v)
		{
		}
	}

	// mp4a
	public class Mp4AudioSampleEntry : AudioSampleEntry
	{
		public Mp4AudioSampleEntry() : base(SampleEntryCode.mp4a)
		{
		}
	}

	// mp4s
	public class MpegSampleEntry : SampleEntry
	{
		public MpegSampleEntry() : base(SampleEntryCode.mp4s)
		{
		}
	}
}
