using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.6.6 Edit List Box
	/// </summary>
	public class EditListBox : FullBox
	{
		public UInt32 EntryCount { get; set; }

		public class Entry
		{
			public UInt64 SegmentDuration { get; set; }
			public UInt64 MediaTime { get; set; }
			public Int16 MediaRateInteger { get; set; }
			public Int16 MediaRateFraction { get; set; }
		}
		public List<Entry> Entries { get; private set; }

		public EditListBox() : base(BoxType.elst)
		{
			Entries = new List<Entry>();
		}
	}
}
