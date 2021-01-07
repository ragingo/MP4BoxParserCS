using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.6.1 Time to Sample Boxes
    /// 8.6.1.3 Composition Time to Sample Box
    /// </summary>
    public class CompositionOffsetBox : FullBox
    {
        public UInt32 EntryCount { get; set; }

        public class Entry
        {
            public UInt32 SampleCount { get; set; }
            public UInt32 SampleOffset { get; set; } // version=1 の場合は signed int(32)
        }
        public List<Entry> Entries { get; private set; }

        public CompositionOffsetBox() : base(BoxType.ctts)
        {
            Entries = new List<Entry>();
        }
    }
}
