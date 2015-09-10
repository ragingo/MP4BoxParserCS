using System;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.4.5.4 Hint Media Header Box
	/// </summary>
	public class HintMediaHeaderBox : FullBox
	{
		public UInt16 MaxPduSize { get; set; }
		public UInt16 AvgPduSize { get; set; }
		public UInt32 MaxBitrate { get; set; }
		public UInt32 AvgBitrate { get; set; }
		public UInt32 Reserved { get; set; }

		public HintMediaHeaderBox() : base(BoxType.hmhd)
		{
		}
	}
}
