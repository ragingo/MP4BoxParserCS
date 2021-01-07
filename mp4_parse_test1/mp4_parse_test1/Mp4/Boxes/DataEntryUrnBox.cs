namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.7.2 Data Reference Box
    /// DataEntryUrnBox
    /// </summary>
    public class DataEntryUrnBox : FullBox, IDataEntryBox
    {
        public string Name { get; set; }
        public string Location { get; set; }

        public DataEntryUrnBox() : base(BoxType.urn)
        {
        }
    }
}
