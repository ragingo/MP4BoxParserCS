using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mp4_parse_test1
{
    public static class StringUtil
    {
        public static string FromBinary(UInt32 binary, Encoding encoding)
        {
            byte[] bytes =
                new[] {
                    (byte)((binary & 0xff000000) >> 24),
                    (byte)((binary & 0x00ff0000) >> 16),
                    (byte)((binary & 0x0000ff00) >>  8),
                    (byte)((binary & 0x000000ff) >>  0),
                };
            return encoding.GetString(bytes);
        }

        public static string FromBinary(UInt32 binary)
        {
            return FromBinary(binary, Encoding.ASCII);
        }

        public static string FromBinary(IEnumerable<byte> binary, Encoding encoding)
        {
            byte[] bytes = binary.Reverse().Select(x => x == '\0' ? (byte)' ' : x).ToArray();
            return encoding.GetString(bytes);
        }

        public static string FromBinary(IEnumerable<byte> binary)
        {
            return FromBinary(binary, Encoding.ASCII);
        }
    }
}
