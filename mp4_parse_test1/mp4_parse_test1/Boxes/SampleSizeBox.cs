using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.7.3 Sample Size Boxes
	/// 8.7.3.2 Sample Size Box
	/// </summary>
	public class SampleSizeBox : FullBox
	{
		public UInt32 SampleSize { get; set; }
		public UInt32 SampleCount { get; set; }

		public class Entry
		{
			public UInt32 Size { get; set; }
		}
		public List<Entry> Entries { get; private set; }

		public SampleSizeBox() : base(BoxType.stsz)
		{
			Entries = new List<Entry>();
		}
	}
}
