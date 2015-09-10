using System;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.3.4 Track Group Box
	/// この Box は Box クラス を継承しない
	/// </summary>
	public class TrackGroupBox : BoxBase
	{
		public TrackGroupBox() : base(BoxType.trgr)
		{
		}
	}

	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.3.4 Track Group Box
	/// </summary>
	public class TrackGroupTypeBox : FullBox
	{
		public UInt32 TrackGroupID { get; set; }

		public TrackGroupTypeBox(BoxType type) : base(type)
		{
		}
	}
}
