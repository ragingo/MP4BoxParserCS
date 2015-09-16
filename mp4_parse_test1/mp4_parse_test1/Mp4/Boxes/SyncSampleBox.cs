using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.6.2 Sync Sample Box
	/// </summary>
	public class SyncSampleBox : FullBox
	{
		public UInt32 EntryCount { get; set; }

		public class Entry
		{
			public UInt32 SampleNumber { get; set; }
		}
		public List<Entry> Entries { get; private set; }

		public SyncSampleBox() : base(BoxType.stss)
		{
			Entries = new List<Entry>();
		}
	}
}
