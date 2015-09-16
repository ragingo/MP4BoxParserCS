using System;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.2.2 Movie Header Box
	/// </summary>
	public class MovieHeaderBox : FullBox
	{
		public DateTime CreationTime { get; set; }
		public DateTime ModificationTime { get; set; }
		public UInt32 TimeScale { get; set; }
		public UInt64 Duration { get; set; }
		public Double Rate { get; set; }
		public Single Volume { get; set; }
		public UInt16 Reserved { get; set; }
		public UInt32[] Reserved2 { get; private set; }
		public UInt32[] Matrix { get; private set; }
		public UInt32[] PreDefined { get; private set; }
		public UInt32 NextTrackId { get; set; }

		public MovieHeaderBox() : base(BoxType.mvhd)
		{
			Reserved2 = new UInt32[2];
			Matrix = new UInt32[9];
			PreDefined = new UInt32[6];
		}
	}
}
