using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.6.3 Shadow Sync Sample Box
    /// </summary>
    public class ShadowSyncSampleBox : FullBox
    {
        public UInt32 EntryCount { get; set; }

        public class Entry
        {
            public UInt32 ShadowedSampleNumber { get; set; }
            public UInt32 SyncSampleNumber { get; set; }
        }
        public List<Entry> Entries { get; private set; }

        public ShadowSyncSampleBox() : base(BoxType.stsh)
        {
            Entries = new List<Entry>();
        }
    }
}
