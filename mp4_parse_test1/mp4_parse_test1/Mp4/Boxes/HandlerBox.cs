using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.4.3 Handler Reference Box
    /// </summary>
    public class HandlerBox : FullBox
    {
        public UInt32 PreDefined { get; set; }
        public HandlerType HandlerType { get; set; }
        public UInt32[] Reserved { get; private set; }
        public string Name { get; set; }

        public HandlerBox() : base(BoxType.hdlr)
        {
            Reserved = new UInt32[3];
        }
    }
}
