using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.1.1 Media Data Box
	/// </summary>
	public class MediaDataBox : Box
	{
		public IEnumerable<byte> Data { get; set; }

		public MediaDataBox() : base(BoxType.mdat)
		{
		}
	}
}
