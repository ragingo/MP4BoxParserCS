using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// CleanApertureBox
    /// </summary>
    public class CleanApertureBox : Box
    {
        public UInt32 CleanApertureWidthN { get; set; }
        public UInt32 CleanApertureWidthD { get; set; }
        public UInt32 CleanApertureHeightN { get; set; }
        public UInt32 CleanApertureHeightD { get; set; }
        public UInt32 HorizOffN { get; set; }
        public UInt32 HorizOffD { get; set; }
        public UInt32 VertOffN { get; set; }
        public UInt32 VertOffD { get; set; }

        public CleanApertureBox() : base(BoxType.clap)
        {
        }
    }
}
