using System;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) FullBox
	/// </summary>
	public class FullBox : Box
	{
		public byte Version { get; set; }
		public UInt32 Flags { get; set; }

		public FullBox(BoxType type) : base(type)
		{
		}
	}
}
