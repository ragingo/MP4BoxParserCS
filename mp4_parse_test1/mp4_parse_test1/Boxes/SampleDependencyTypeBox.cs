using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.6.4 Independent and Disposable Samples Box
	/// </summary>
	public class SampleDependencyTypeBox : FullBox
	{
		public class Sample
		{
			public bool IsLeading { get; set; } // 2 bits
			public byte DependsOn { get; set; } // 2 bits
			public byte IsDependedOn { get; set; } // 2 bits
			public byte HasRedundancy { get; set; } // 2 bits
		}

		public List<Sample> Samples { get; private set; }


		public SampleDependencyTypeBox() : base(BoxType.sdtp)
		{
			Samples = new List<Sample>();
		}
	}
}
