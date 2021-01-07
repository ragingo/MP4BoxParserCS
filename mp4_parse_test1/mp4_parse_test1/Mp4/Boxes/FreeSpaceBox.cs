using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.1.2 Free Space Box
    /// </summary>
    public class FreeSpaceBox : Box
    {
        public IEnumerable<byte> Data { get; set; }

        public FreeSpaceBox() : base(BoxType.free)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.1.2 Free Space Box
    /// </summary>
    public class SkipBox : Box
    {
        public IEnumerable<byte> Data { get; set; }

        public SkipBox() : base(BoxType.skip)
        {
        }
    }
}
