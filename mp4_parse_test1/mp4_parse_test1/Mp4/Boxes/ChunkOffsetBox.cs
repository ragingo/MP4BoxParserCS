using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.7.5 Chunk Offset Box
    /// ChunkOffsetBox
    /// </summary>
    public class ChunkOffsetBox : FullBox
    {
        public UInt32 EntryCount { get; set; }

        public class Entry
        {
            public UInt32 ChunkOffset { get; set; }
        }
        public List<Entry> Entries { get; private set; }

        public ChunkOffsetBox() : base(BoxType.stco)
        {
            Entries = new List<Entry>();
        }
    }
}
