namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.4.4 Media Information Box
    /// </summary>
    public class MediaInformationBox : Box
    {
        public MediaInformationBox() : base(BoxType.minf)
        {
        }
    }
}
