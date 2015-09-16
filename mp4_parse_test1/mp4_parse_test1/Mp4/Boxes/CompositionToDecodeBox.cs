using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.6.1 Time to Sample Boxes
	/// 8.6.1.4 Composition to Decode Box
	/// </summary>
	public class CompositionToDecodeBox : FullBox
	{
		public Int32 CompositionToDTSShift { get; set; }
		public Int32 LeastDecodeToDisplayDelta { get; set; }
		public Int32 GreatestDecodeToDisplayDelta { get; set; }
		public Int32 CompositionStartTime { get; set; }
		public Int32 CompositionEndTime { get; set; }

		public CompositionToDecodeBox() : base(BoxType.cslg)
		{
		}
	}
}
