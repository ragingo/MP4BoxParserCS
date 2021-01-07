using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.4.5.3 Sound Media Header Box
    /// </summary>
    public class SoundMediaHeaderBox : FullBox
    {
        public UInt16 Balance { get; set; }
        public UInt16 Reserved { get; set; }

        public SoundMediaHeaderBox() : base(BoxType.smhd)
        {
        }
    }
}
