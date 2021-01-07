using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.7.5 Chunk Offset Box
    /// ChunkLargeOffsetBox
    /// </summary>
    public class ChunkLargeOffsetBox : FullBox
    {
        public UInt32 EntryCount { get; set; }

        public class Entry
        {
            public UInt64 ChunkOffset { get; set; }
        }
        public List<Entry> Entries { get; private set; }

        public ChunkLargeOffsetBox() : base(BoxType.co64)
        {
        }
    }
}
