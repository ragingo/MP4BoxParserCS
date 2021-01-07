namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.3.1 Track Box
    /// </summary>
    public class TrackBox : Box
    {
        public TrackBox() : base(BoxType.trak)
        {
        }
    }
}
