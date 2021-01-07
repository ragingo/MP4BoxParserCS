using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// </summary>
    public class SampleDescriptionBox : FullBox
    {
        public UInt32 EntryCount { get; set; }

        public class Entry
        {
            public SampleEntry SampleEntry { get; set; }
        }
        public List<Entry> Entries { get; private set; }

        public SampleDescriptionBox() : base(BoxType.stsd)
        {
            Entries = new List<Entry>();
        }
    }
}
