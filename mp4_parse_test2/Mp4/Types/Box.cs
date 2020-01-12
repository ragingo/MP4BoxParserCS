using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace mp4_parse_test2.Mp4.Types
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    struct Box
    {
        [FieldOffset(0)] public int Size;
        [FieldOffset(4)] public BoxType Type;

        public Box(ReadOnlySpan<byte> span)
        {
            Size = (int)BinaryPrimitives.ReadUInt32BigEndian(span.Slice(0, 4));
            Type = (BoxType)BinaryPrimitives.ReadUInt32BigEndian(span.Slice(4, 4));
        }

        public override string ToString()
        {
            return BoxUtils.ToString(this);
        }
    }
}
