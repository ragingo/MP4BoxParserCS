using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.3.2 Track Header Box
    /// </summary>
    public class TrackHeaderBox : FullBox
    {
        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }
        public UInt32 TrackId { get; set; }
        public UInt32 Reserved { get; set; }
        public UInt64 Duration { get; set; }
        public UInt32[] Reserved2 { get; private set; }
        public UInt16 Layer { get; set; }
        public UInt16 AlternateGroup { get; set; }
        public UInt16 Volume { get; set; }
        public UInt16 Reserved3 { get; set; }
        public UInt32[] Matrix { get; set; }
        public UInt32 Width { get; set; }
        public UInt32 Height { get; set; }

        public TrackHeaderBox() : base(BoxType.tkhd)
        {
            Reserved2 = new UInt32[2];
            Matrix = new UInt32[9];
        }
    }
}
