using System;
using mp4_parse_test2.Mp4.Types;

namespace mp4_parse_test2.Mp4
{
    static class BoxUtils
    {
        public static string GetStringFromUInt32(uint value)
        {
            Span<char> chars = stackalloc char[4];
            chars[0] = (char)(value >> 24 & 0xff);
            chars[1] = (char)(value >> 16 & 0xff);
            chars[2] = (char)(value >> 8 & 0xff);
            chars[3] = (char)(value & 0xff);
            return new string(chars);
        }

        public static string ToString(Box box)
        {
            return string.Format("type: {0}, size: 0x{1:x2} ({1:#,#})", GetStringFromUInt32((uint)box.Type), box.Size);
        }

        public static string ToString(FileTypeBox box)
        {
            return string.Format("type: {0}, size: 0x{1:x2} ({1:#,#})", GetStringFromUInt32((uint)box.Box.Type), box.Box.Size);
        }
    }
}
