using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.6.1 Time to Sample Boxes
    /// 8.6.1.2 Decoding Time to Sample Box
    /// </summary>
    public class TimeToSampleBox : FullBox
    {
        public UInt32 EntryCount { get; set; }

        public class Entry
        {
            public UInt32 SampleCount { get; set; }
            public UInt32 SampleDelta { get; set; }
        }
        public List<Entry> Entries { get; private set; }

        public TimeToSampleBox() : base(BoxType.stts)
        {
            Entries = new List<Entry>();
        }
    }
}
