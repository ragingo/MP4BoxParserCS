using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace mp4_parse_test2.Mp4.Types
{
    // ftyp
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    struct FileTypeBox
    {
        [FieldOffset(0)] public Box Box;
        [FieldOffset(8)] public uint MajorBland;
        [FieldOffset(12)] public uint MinorVersion;

        public FileTypeBox(ref Box box, ReadOnlySpan<byte> span)
        {
            Box = box;
            MajorBland = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(0, 4));
            MinorVersion = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(4, 4));
        }

        public override string ToString()
        {
            return BoxUtils.ToString(this);
        }
    }
}
