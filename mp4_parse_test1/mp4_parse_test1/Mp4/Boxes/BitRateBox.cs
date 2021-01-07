using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// BitRateBox
    /// </summary>
    public class BitRateBox : Box
    {
        public UInt32 BufferSizeDB { get; set; }
        public UInt32 MaxBitrate { get; set; }
        public UInt32 AvgBitrate { get; set; }

        public BitRateBox() : base(BoxType.btrt)
        {
        }
    }
}
