using System;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
	/// </summary>
	public class SampleDescriptionBox : FullBox
	{
		public UInt32 EntryCount { get; set; }

		public SampleDescriptionBox() : base(BoxType.stsd)
		{
		}
	}
}
