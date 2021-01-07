using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.7.4 Sample To Chunk Box
    /// </summary>
    public class SampleToChunkBox : FullBox
    {
        public UInt32 EntryCount { get; set; }

        public class Entry
        {
            public UInt32 FirstChunk { get; set; }
            public UInt32 SamplesPerChunk { get; set; }
            public UInt32 SampleDescriptionIndex { get; set; }
        }
        public List<Entry> Entries { get; private set; }

        public SampleToChunkBox() : base(BoxType.stsc)
        {
            Entries = new List<Entry>();
        }
    }
}
