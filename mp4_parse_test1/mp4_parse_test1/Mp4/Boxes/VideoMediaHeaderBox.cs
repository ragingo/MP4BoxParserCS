using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.4.5.2 Video Media Header Box
    /// </summary>
    public class VideoMediaHeaderBox : FullBox
    {
        public UInt16 GraphicsMode { get; set; }
        public UInt16[] OpColor { get; private set; }

        public VideoMediaHeaderBox() : base(BoxType.vmhd)
        {
            OpColor = new UInt16[3];
        }
    }
}
