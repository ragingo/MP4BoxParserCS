using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.1.3 Progressive Download Information Box
    /// </summary>
    public class ProgressiveDownloadInfoBox : FullBox
    {
        public class Info
        {
            public UInt32 Rate { get; set; }
            public UInt32 InitialDelay { get; set; }
        }

        public List<Info> Infos { get; private set; }

        public ProgressiveDownloadInfoBox() : base(BoxType.pdin)
        {
            Infos = new List<Info>();
        }
    }
}
