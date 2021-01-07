namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.2.1 Movie Box
    /// </summary>
    public class MovieBox : Box
    {
        public MovieBox() : base(BoxType.moov)
        {
        }
    }
}
