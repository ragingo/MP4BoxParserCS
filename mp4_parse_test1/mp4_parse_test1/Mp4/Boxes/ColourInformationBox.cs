using System;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
	/// ColourInformationBox
	/// </summary>
	public class ColourInformationBox : Box
	{
		public ColourType ColourType { get; set; }
		public UInt16 ColourPrimaries { get; set; }
		public UInt16 TransferCharacteristics { get; set; }
		public UInt16 MatrixCoefficients { get; set; }
		public byte FullRangeFlag { get; set; } // 1 bit
		public byte Reserved { get; set; } // 7 bit
		//public ICC_profile ICC_profile { get; private set; }

		public ColourInformationBox() : base(BoxType.colr)
		{
			// TODO: ICC_profile を定義する
			//ICC_profile = new ICC_profile();
		}
	}
}
