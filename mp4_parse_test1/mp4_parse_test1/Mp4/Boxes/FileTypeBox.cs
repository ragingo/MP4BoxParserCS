using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 4.3 File Type Box
    /// </summary>
    public class FileTypeBox : Box
    {
        public UInt32 MajorBrand { get; set; }
        public UInt32 MinorVersion { get; set; }
        public List<Brand> CompatibleBrands { get; private set; }

        public FileTypeBox() : base(BoxType.ftyp)
        {
            CompatibleBrands = new List<Brand>();
        }
    }
}
