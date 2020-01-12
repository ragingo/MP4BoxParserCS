using System.Runtime.InteropServices;

namespace mp4_parse_test2.Mp4.Types
{
    static class BoxSizes
    {
        public static readonly int box = Marshal.SizeOf<Box>();
        public static readonly int ftyp = Marshal.SizeOf<FileTypeBox>();
    }
}
