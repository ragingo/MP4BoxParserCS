using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.4.2 Media Header Box
    /// </summary>
    public class MediaHeaderBox : FullBox
    {
        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }
        public UInt32 TimeScale { get; set; }
        public UInt64 Duration { get; set; }
        public byte Pad { get; set; }
        public string Language { get; set; }
        public UInt16 PreDefined { get; set; }

        public MediaHeaderBox() : base(BoxType.mdhd)
        {
        }
    }
}
