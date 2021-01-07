using System;

namespace mp4_parse_test1.Aac
{
    /// <summary>
    /// ISO/IEC 13818-7:2004(E) 6.2.1 Fixed Header of ADTS
    /// </summary>
    public class AdtsHeader
    {
        public UInt16 SyncWord { get; set; }
        public byte ID { get; set; }
        public byte Layer { get; set; }
        public byte ProtectionAbsent { get; set; }
        public byte Profile { get; set; }
        public byte SamplingFrequencyIndex { get; set; }
        public byte PrivateBit { get; set; }
        public byte ChannelConfiguration { get; set; }
        public byte OriginalOrCopy { get; set; }
        public byte Home { get; set; }
    }
}
