namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.7.1 Data Information Box
	/// </summary>
	public class DataInformationBox : Box
	{
		public DataInformationBox() : base(BoxType.dinf)
		{
		}
	}
}
