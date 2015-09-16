namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.7.2 Data Reference Box
	/// DataEntryUrlBox
	/// </summary>
	public class DataEntryUrlBox : FullBox, IDataEntryBox
	{
		public string Location { get; set; }

		public DataEntryUrlBox() : base(BoxType.url)
		{
		}
	}
}
