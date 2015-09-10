namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.4.5.5 Null Media Header Box
	/// </summary>
	public class NullMediaHeaderBox : FullBox
	{
		public NullMediaHeaderBox() : base(BoxType.nmhd)
		{
		}
	}
}
