using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.7.3 Sample Size Boxes
    /// 8.7.3.3 Compact Sample Size Box
    /// </summary>
    public class CompactSampleSizeBox : FullBox
    {
        public UInt32 Reserved { get; set; } // 24 bits
        public byte FieldSize { get; set; }
        public UInt32 SampleCount { get; set; }

        public class Sample
        {
            public UInt16 EntrySize { get; set; }
        }
        public List<Sample> Samples { get; private set; }

        public CompactSampleSizeBox() : base(BoxType.stz2)
        {
            Samples = new List<Sample>();
        }
    }
}
