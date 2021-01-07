
namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// URIBox
    /// </summary>
    public class URIBox : FullBox
    {
        public string TheUri { get; set; }

        public URIBox() : base(BoxType.uri)
        {
        }
    }
}
