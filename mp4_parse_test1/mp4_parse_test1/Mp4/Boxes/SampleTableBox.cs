namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.1 Sample Table Box
    /// </summary>
    public class SampleTableBox : Box
    {
        public SampleTableBox() : base(BoxType.stbl)
        {
        }
    }
}
