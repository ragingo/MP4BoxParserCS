using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// PixelAspectRatioBox
    /// </summary>
    public class PixelAspectRatioBox : Box
    {
        public UInt32 HSpacing { get; set; }
        public UInt32 VSpacing { get; set; }

        public PixelAspectRatioBox() : base(BoxType.pasp)
        {
        }
    }
}
