using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.5.3 Degradation Priority Box
	/// </summary>
	public class DegradationPriorityBox : FullBox
	{
		public List<UInt16> Priorities { get; private set; }

		public DegradationPriorityBox() : base(BoxType.stdp)
		{
			Priorities = new List<UInt16>();
		}
	}
}
